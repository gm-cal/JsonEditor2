using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;
using Services;

namespace Controls{
    public partial class TextEdit : UserControl{
        private ListBox lineList => LinesList;
        public event EventHandler<int>? LineControlRequested;
        private bool internalChange = false;
        private readonly ObservableCollection<TextLine> lines = new ObservableCollection<TextLine>();

        public ObservableCollection<TextLine> Lines => lines;

        public TextEdit(){
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            EditorSettings.Changed += (_, _) => UpdateLineVisibility();
            UpdateLineVisibility();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue is TextEditorViewModel vm) {
                Lines.Clear();
                string[] raw = vm.Text.Replace("\r\n", "\n").Split('\n');
                for (int i = 0; i < raw.Length; i++) {
                    Lines.Add(new TextLine { LineNumber = i + 1, Text = raw[i] });
                }

                vm.PropertyChanged += (s, evt) => {
                    if (evt.PropertyName == nameof(TextEditorViewModel.Text)) {
                        // Optional: Update UI when VM changes from outside
                    }
                };
            }
        }

        private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e){
            if(e.PropertyName == nameof(TextEditorViewModel.Text) && sender is TextEditorViewModel vm){
                SetText(vm.Text);
            }
        }

        private void SetText(string text){
            internalChange = true;
            lines.Clear();
            string[] raw = text.Replace("\r\n", "\n").Split('\n');
            int num = 1;
            foreach(string l in raw){
                lines.Add(new TextLine{ LineNumber = num++, Text = l });
            }
            internalChange = false;
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
            UpdateVmText();
        }

        private void Renumber(){
            for(int i = 0; i < lines.Count; i++) lines[i].LineNumber = i + 1;
        }

        public (IList<TextLine> Lines, int StartLine, int EndLine) GetSelectedLineRange(){
            int startChar = editorBox.SelectionStart;
            int endChar = startChar + editorBox.SelectionLength;
            int start = editorBox.GetLineIndexFromCharacterIndex(startChar);
            int end = editorBox.GetLineIndexFromCharacterIndex(endChar);
            return (lines, start, end);
        }

        private void OnLineNumberMouseDown(object sender, MouseButtonEventArgs e){
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
                if(sender is ListBoxItem item && item.DataContext is TextLine line){
                    LineControlRequested?.Invoke(this, line.LineNumber);
                    e.Handled = true;
                }
            }
        }

        private void UpdateLineVisibility(){
            lineList.Visibility = EditorSettings.ShowLineNumbers ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
