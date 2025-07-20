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
    public partial class TextEdit : UserControl {
        // List displaying the editor lines
        private ListBox lineList => Editor;
        public event EventHandler<int>? LineControlRequested;
        private bool internalChange = false;
        private readonly ObservableCollection<TextLine> lines = new();

        public ObservableCollection<TextLine> Lines => lines;

        public TextEdit(){
            InitializeComponent();
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
                UpdateVmText();
            }
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
    }
}
