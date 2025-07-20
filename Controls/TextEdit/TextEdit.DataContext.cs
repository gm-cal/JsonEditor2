using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ViewModels;

namespace Controls{
    public partial class TextEdit{
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e){
            if(e.OldValue is TextEditorViewModel oldVm)
                oldVm.PropertyChanged -= OnVmPropertyChanged;

            if(e.NewValue is TextEditorViewModel vm){
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

        private void OnLineChanged(object? sender, PropertyChangedEventArgs e){
            if(e.PropertyName == nameof(TextLine.Text))
                ScheduleVmUpdate();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e){
            updateTimer.Stop();
            UpdateVmText();
        }

        private void ScheduleVmUpdate(){
            if(internalChange) return;
            updateTimer.Stop();
            updateTimer.Start();
        }

        private void UpdateVmText(){
            if(internalChange) return;
            if(DataContext is TextEditorViewModel vm)
                vm.Text = string.Join(Environment.NewLine, lines.Select(l => l.Text));
        }

        private void UpdateLineVisibility(){
            lineList.Items.Refresh();
        }
    }
}
