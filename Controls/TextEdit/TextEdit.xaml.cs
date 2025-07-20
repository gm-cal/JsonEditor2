using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ViewModels;
using Services;

namespace Controls{
    public partial class TextEdit : UserControl {
        // List displaying the editor lines
        private ListBox lineList => Editor;
        public event EventHandler<int>? LineControlRequested;
        private bool internalChange = false;
        private readonly DispatcherTimer updateTimer = new();
        private readonly ObservableCollection<TextLine> lines = new();

        public ObservableCollection<TextLine> Lines => lines;

        public TextEdit(){
            InitializeComponent();
            updateTimer.Interval = TimeSpan.FromMilliseconds(300);
            updateTimer.Tick += UpdateTimer_Tick;
            DataContextChanged += OnDataContextChanged;
            EditorSettings.Changed += (_, _) => UpdateLineVisibility();
            UpdateLineVisibility();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue is TextEditorViewModel oldVm)
                oldVm.PropertyChanged -= OnVmPropertyChanged;

            if (e.NewValue is TextEditorViewModel vm) {
                SetText(vm.Text);
                vm.PropertyChanged += OnVmPropertyChanged;
            }
        }

        private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e){
            if(e.PropertyName == nameof(TextEditorViewModel.Text) && sender is TextEditorViewModel vm){
                SetText(vm.Text);
            }
        }

        private void SetText(string text){
            internalChange = true;
            updateTimer.Stop();
            foreach(var l in lines)
                l.PropertyChanged -= OnLineChanged;

            lines.Clear();

            string[] raw = text.Replace("\r\n", "\n").Split('\n');
            int num = 1;
            foreach(string l in raw){
                var tl = new TextLine { LineNumber = num++, Text = l };
                tl.PropertyChanged += OnLineChanged;
                lines.Add(tl);
            }
            internalChange = false;
        }

        private void OnLineChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(TextLine.Text))
            {
                ScheduleVmUpdate();
            }
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            updateTimer.Stop();
            UpdateVmText();
        }

        private void ScheduleVmUpdate()
        {
            if(internalChange) return;
            updateTimer.Stop();
            updateTimer.Start();
        }

        private void UpdateVmText(){
            if(internalChange) return;
            if(DataContext is TextEditorViewModel vm){
                vm.Text = string.Join(Environment.NewLine, lines.Select(l => l.Text));
            }
        }

        public void IndentSelection(){
            ModifySelection(true);
        }

        public void UnindentSelection(){
            ModifySelection(false);
        }

        private void ModifySelection(bool indent){
            var (currentLines, start, end) = GetSelectedLineRange();
            IndentService.ModifySelection(currentLines, start, end, indent);
            Renumber();
            ScheduleVmUpdate();
        }

        private void Renumber(){
            for(int i = 0; i < lines.Count; i++) lines[i].LineNumber = i + 1;
        }

        public (IList<TextLine> Lines, int StartLine, int EndLine) GetSelectedLineRange(){
            if (lineList.SelectedItems.Count == 0) {
                return (lines, lineList.SelectedIndex, lineList.SelectedIndex);
            }

            int start = lineList.Items.IndexOf(lineList.SelectedItems[0]);
            int end = lineList.Items.IndexOf(lineList.SelectedItems[^1]);
            if (start > end) (start, end) = (end, start);
            return (lines, start, end);
        }

        private void OnLineNumberMouseDown(object sender, MouseButtonEventArgs e){
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
                if(sender is FrameworkElement element && element.DataContext is TextLine line){
                    LineControlRequested?.Invoke(this, line.LineNumber);
                    e.Handled = true;
                }
            }
        }

        private void UpdateLineVisibility(){
            lineList.Items.Refresh();
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(sender is not TextBox tb) return;

            if(e.Key == Key.Enter)
            {
                InsertNewLine(tb);
                e.Handled = true;
            }
            else if(e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if(Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    if(text.Contains('\n'))
                    {
                        PasteMultiline(tb, text);
                        e.Handled = true;
                    }
                }
            }
        }

        private void OnTextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if(e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                string text = e.DataObject.GetData(DataFormats.UnicodeText) as string ?? string.Empty;
                if(text.Contains('\n') && sender is TextBox tb)
                {
                    PasteMultiline(tb, text);
                    e.CancelCommand();
                    e.Handled = true;
                }
            }
        }

        private void InsertNewLine(TextBox tb)
        {
            int index = lineList.ItemContainerGenerator.IndexFromContainer(lineList.ContainerFromElement(tb));
            if(index < 0) return;

            TextLine line = lines[index];
            int caret = tb.CaretIndex;
            string before = line.Text.Substring(0, Math.Min(caret, line.Text.Length));
            string after = line.Text.Substring(Math.Min(caret, line.Text.Length));

            line.Text = before;
            TextLine newLine = new TextLine { Text = after };
            newLine.PropertyChanged += OnLineChanged;
            lines.Insert(index + 1, newLine);
            Renumber();
            ScheduleVmUpdate();

            lineList.UpdateLayout();
            if(lineList.ItemContainerGenerator.ContainerFromIndex(index + 1) is ListBoxItem item)
            {
                if(FindTextBox(item) is TextBox nextBox)
                {
                    nextBox.Focus();
                    nextBox.CaretIndex = 0;
                }
            }
        }

        private void PasteMultiline(TextBox tb, string text)
        {
            int index = lineList.ItemContainerGenerator.IndexFromContainer(lineList.ContainerFromElement(tb));
            if(index < 0) return;

            text = text.Replace("\r\n", "\n");
            string[] parts = text.Split('\n');

            TextLine line = lines[index];
            int caret = tb.CaretIndex;
            string before = line.Text.Substring(0, Math.Min(caret, line.Text.Length));
            string after = line.Text.Substring(Math.Min(caret, line.Text.Length));

            line.Text = before + parts[0];

            for(int i = 1; i < parts.Length; i++)
            {
                TextLine nl = new TextLine { Text = parts[i] };
                nl.PropertyChanged += OnLineChanged;
                lines.Insert(index + i, nl);
            }

            lines[index + parts.Length - 1].Text += after;
            Renumber();
            ScheduleVmUpdate();

            lineList.UpdateLayout();
            if(lineList.ItemContainerGenerator.ContainerFromIndex(index + parts.Length - 1) is ListBoxItem item)
            {
                if(FindTextBox(item) is TextBox nextBox)
                {
                    nextBox.Focus();
                    nextBox.CaretIndex = parts[^1].Length;
                }
            }
        }

        private static TextBox? FindTextBox(DependencyObject parent)
        {
            for(int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if(child is TextBox tb) return tb;
                var result = FindTextBox(child);
                if(result != null) return result;
            }
            return null;
        }
    }
}
