using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using ViewModels;
using Services;

namespace Controls{
    public partial class TextEdit : UserControl{
        private TextBox editorControl => Editor;
        private ListBox lineNumbers => LineNumbers;
        private readonly Stack<string> undoStack = new Stack<string>();
        private readonly Stack<string> redoStack = new Stack<string>();
        private string lastText = string.Empty;
        private bool internalChange = false;
        private ScrollViewer? editorScroll;
        private ScrollViewer? lineScroll;
        public static readonly DependencyProperty TextServiceProperty =
            DependencyProperty.Register(nameof(TextService), typeof(ITextService), typeof(TextEdit));

        public ITextService? TextService{
            get => (ITextService?)GetValue(TextServiceProperty);
            set => SetValue(TextServiceProperty, value);
        }

        public TextEdit(){
            InitializeComponent();
            editorControl.PreviewKeyDown += OnPreviewKeyDown;
            editorControl.TextChanged += OnTextChanged;
            EditorSettings.Changed += (_, _) => UpdateLineNumberVisibility();
            Loaded += (_, _) => InitializeScrolling();
            UpdateLineNumbers();
        }

        public void IndentSelection(){
            ModifySelection(true);
        }

        public void UnindentSelection(){
            ModifySelection(false);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e){
            if(EditorSettings.IndentGesture.Matches(null, e)){
                IndentSelection();
                e.Handled = true;
            }else if(EditorSettings.UnindentGesture.Matches(null, e)){
                UnindentSelection();
                e.Handled = true;
            }else if(e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
                Undo();
                e.Handled = true;
            }else if(e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
                Redo();
                e.Handled = true;
            }
        }

        private void ModifySelection(bool indent){
            PushUndo();
            internalChange = true;
            TextService?.ModifySelection(
                editorControl.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None),
                indent,
                out string[] modifiedLines);
            internalChange = false;
            UpdateLineNumbers();
        }

        private void OnDrop(object sender, DragEventArgs e){
            if(DataContext is TextEditorViewModel vm){
                if(e.Data.GetDataPresent(DataFormats.FileDrop)){
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if(files.Length > 0){
                        vm.FilePath = files[0];
                        vm.OpenCommand.Execute(null);
                    }
                }
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e){
            if(!internalChange){
                undoStack.Push(lastText);
                redoStack.Clear();
            }
            lastText = editorControl.Text;
            UpdateLineNumbers();
        }

        private void PushUndo(){
            undoStack.Push(editorControl.Text);
            redoStack.Clear();
        }

        private void Undo(){
            if(undoStack.Count > 0){
                internalChange = true;
                string current = editorControl.Text;
                string prev = undoStack.Pop();
                redoStack.Push(current);
                editorControl.Text = prev;
                internalChange = false;
                UpdateLineNumbers();
            }
        }

        private void Redo(){
            if(redoStack.Count > 0){
                internalChange = true;
                string current = editorControl.Text;
                string next = redoStack.Pop();
                undoStack.Push(current);
                editorControl.Text = next;
                internalChange = false;
                UpdateLineNumbers();
            }
        }

        private void UpdateLineNumbers(){
            int count = editorControl.LineCount;
            List<string> nums = new();
            for(int i = 1; i <= count; i++) nums.Add(i.ToString());
            lineNumbers.ItemsSource = nums;
            UpdateLineNumberVisibility();
            AdjustLineHeights();
        }

        private void UpdateLineNumberVisibility(){
            lineNumbers.Visibility = EditorSettings.ShowLineNumbers ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnLineNumberSelection(object sender, SelectionChangedEventArgs e){
            if(lineNumbers.SelectedItems.Count == 0) return;
            int start = lineNumbers.SelectedIndex;
            int end = start + lineNumbers.SelectedItems.Count - 1;
            if(start < 0) return;
            int startPos = editorControl.GetCharacterIndexFromLineIndex(start);
            int endPos = editorControl.GetCharacterIndexFromLineIndex(end) + editorControl.GetLineLength(end);
            editorControl.SelectionStart = startPos;
            editorControl.SelectionLength = Math.Max(0, endPos - startPos);
        }

        private void InitializeScrolling(){
            editorScroll = FindScrollViewer(editorControl);
            lineScroll = FindScrollViewer(lineNumbers);
            if(editorScroll != null && lineScroll != null){
                editorScroll.ScrollChanged += (_, e) => lineScroll.ScrollToVerticalOffset(e.VerticalOffset);
            }
            AdjustLineHeights();
        }

        private void AdjustLineHeights(){
            Dispatcher.BeginInvoke(new Action(() => {
                if(editorControl.LineCount == 0) return;
                int first = editorControl.GetCharacterIndexFromLineIndex(0);
                Rect r0 = editorControl.GetRectFromCharacterIndex(first);
                Rect r1 = r0;
                if(editorControl.LineCount > 1){
                    int second = editorControl.GetCharacterIndexFromLineIndex(1);
                    r1 = editorControl.GetRectFromCharacterIndex(second);
                }
                double height = r1.Top > r0.Top ? r1.Top - r0.Top : r0.Height;
                Style style = new(typeof(ListBoxItem));
                style.Setters.Add(new Setter(ListBoxItem.HeightProperty, height));
                style.Setters.Add(new Setter(ListBoxItem.MarginProperty, new Thickness(0)));
                lineNumbers.ItemContainerStyle = style;
                lineNumbers.Margin = new Thickness(0, r0.Top, 0, 0);
            }), DispatcherPriority.Loaded);
        }

        private static ScrollViewer? FindScrollViewer(DependencyObject o){
            if(o is ScrollViewer sv) return sv;
            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++){
                var result = FindScrollViewer(VisualTreeHelper.GetChild(o, i));
                if(result != null) return result;
            }
            return null;
        }
    }
}
