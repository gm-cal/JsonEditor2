using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private readonly List<TextLine> lines = new List<TextLine>();

        public TextEdit(){
            InitializeComponent();
            editorControl.PreviewKeyDown += OnPreviewKeyDown;
            editorControl.TextChanged += OnTextChanged;
            EditorSettings.Changed += (_, _) => UpdateLineNumberVisibility();
            UpdateLineData();
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
            var (currentLines, start, end) = GetSelectedLineRange();
            IndentService.ModifySelection(currentLines, start, end, indent);
            editorControl.Text = string.Join(Environment.NewLine, currentLines.Select(l => l.Text));
            editorControl.SelectionStart = editorControl.GetCharacterIndexFromLineIndex(start);
            editorControl.SelectionLength = Math.Max(0, editorControl.GetCharacterIndexFromLineIndex(end) + currentLines[end].Text.Length - editorControl.SelectionStart);
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
            UpdateLineData();
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
            int count = lines.Count;
            List<string> nums = new();
            for(int i = 1; i <= count; i++) nums.Add(i.ToString());
            lineNumbers.ItemsSource = nums;
            UpdateLineNumberVisibility();
        }

        private void UpdateLineNumberVisibility(){
            lineNumbers.Visibility = EditorSettings.ShowLineNumbers ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateLineData(){
            lines.Clear();
            string[] rawLines = editorControl.Text.Replace("\r\n", "\n").Split('\n');
            foreach(string l in rawLines){
                lines.Add(new TextLine{ Text = l });
            }
        }

        public (List<TextLine> Lines, int StartLine, int EndLine) GetSelectedLineRange(){
            int startLine = editorControl.GetLineIndexFromCharacterIndex(editorControl.SelectionStart);
            int endLine = editorControl.GetLineIndexFromCharacterIndex(editorControl.SelectionStart + editorControl.SelectionLength);
            return (lines, startLine, endLine);
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
    }
}
