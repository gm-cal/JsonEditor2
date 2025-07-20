/// <summary>
/// TextEdit コントロールのデータコンテキスト連携機能を提供します。
/// ViewModel とのバインディング、テキストの同期、
/// 行データの管理などを行います。
/// </summary>
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ViewModels;

namespace Controls{
    public partial class TextEdit{
        // DataContext が変更されたときに呼び出され、ViewModel とのイベント連携を管理します。
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e){
            if(e.OldValue is TextEditorViewModel oldVm)
                oldVm.PropertyChanged -= OnVmPropertyChanged;

            if(e.NewValue is TextEditorViewModel vm){
                SetText(vm.Text);
                vm.PropertyChanged += OnVmPropertyChanged;
            }
        }

        // ViewModel のプロパティ変更時に呼び出され、テキスト変更を反映します。
        private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e){
            if(e.PropertyName == nameof(TextEditorViewModel.Text) && sender is TextEditorViewModel vm){
                SetText(vm.Text);
            }
        }

        // 指定されたテキストを行データに分割し、内部状態を更新します。
        private void SetText(string text){
            internalChange = true;
            updateTimer.Stop();
            foreach(TextLine l in lines)
                l.PropertyChanged -= OnLineChanged;

            lines.Clear();

            string[] raw = text.Replace("\r\n", "\n").Split('\n');
            int num = 1;
            foreach(string l in raw){
                TextLine tl = new TextLine { LineNumber = num++, Text = l };
                tl.PropertyChanged += OnLineChanged;
                lines.Add(tl);
            }
            internalChange = false;
        }

        // 行データのテキスト変更時に呼び出され、ViewModel への反映をスケジュールします。
        private void OnLineChanged(object? sender, PropertyChangedEventArgs e){
            if(e.PropertyName == nameof(TextLine.Text))
                ScheduleVmUpdate();
        }

        // タイマーの Tick イベントで呼び出され、ViewModel のテキストを更新します。
        private void UpdateTimer_Tick(object? sender, EventArgs e){
            updateTimer.Stop();
            UpdateVmText();
        }

        // ViewModel へのテキスト更新を遅延実行するためのタイマーを開始します。
        private void ScheduleVmUpdate(){
            if(internalChange) return;
            updateTimer.Stop();
            updateTimer.Start();
        }

        // 行データから ViewModel のテキストを生成し、ViewModel に反映します。
        private void UpdateVmText(){
            if(internalChange) return;
            if(DataContext is TextEditorViewModel vm)
                vm.Text = string.Join(Environment.NewLine, lines.Select(l => l.Text));
        }

        // 行リストの表示を更新します。
        private void UpdateLineVisibility(){
            lineList.Items.Refresh();
        }
    }
}
