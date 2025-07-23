// TextEdit コントロールの TextBox 操作ハンドラを提供します。
// 行番号クリック、テキストボックスのキーハンドリング、
// 複数行貼り付け、新規行挿入などの機能を持ちます。
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;

namespace Controls{
    public partial class TextEdit{
        // 行番号クリック時の処理。Ctrl+クリックで LineControlRequested イベントを発火します。
        private void OnLineNumberMouseDown(object sender, MouseButtonEventArgs e){
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
                if(sender is FrameworkElement element && element.DataContext is TextLine line){
                    LineControlRequested?.Invoke(this, line.LineNumber);
                    e.Handled = true;
                }
            }
        }

        // テキストボックスの PreviewKeyDown イベントハンドラ。
        // Enter で新規行挿入、Ctrl+V で複数行貼り付けを処理します。
        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e){
            if(sender is not TextBox tb) return;

            if(e.Key == Key.Enter){
                InsertNewLine(tb);
                e.Handled = true;
            }else if(e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
                if(Clipboard.ContainsText()){
                    string text = Clipboard.GetText();
                    if(text.Contains('\n')){
                        PasteMultiline(tb, text);
                        e.Handled = true;
                    }
                }
            }
        }

        // テキストボックスへの貼り付け時の処理。複数行の場合は独自貼り付けを行います。
        private void OnTextBoxPasting(object sender, DataObjectPastingEventArgs e){
            if(e.DataObject.GetDataPresent(DataFormats.UnicodeText)){
                string text = e.DataObject.GetData(DataFormats.UnicodeText) as string ?? string.Empty;
                if(text.Contains('\n') && sender is TextBox tb){
                    PasteMultiline(tb, text);
                    e.CancelCommand();
                    e.Handled = true;
                }
            }
        }

        // テキストボックスの PreviewMouseLeftButtonDown イベントハンドラ。
        // フォーカスがない場合にフォーカスを設定します。
        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e){
            if(sender is TextBox tb && !tb.IsKeyboardFocusWithin){
                tb.Focus();
            }
        }

        // 指定位置に新規行を挿入します。
        private void InsertNewLine(TextBox tb){
            int index = lineList.ItemContainerGenerator.IndexFromContainer(lineList.ContainerFromElement(tb));
            if(index < 0) return;

            int caret = tb.CaretIndex;
            if(DataContext is TextEditorViewModel vm){
                vm.LineService.InsertNewLine(lines, index, caret);
                lines[index + 1].PropertyChanged += OnLineChanged;
                Renumber();
                ScheduleVmUpdate();
            }

            lineList.UpdateLayout();
            if(lineList.ItemContainerGenerator.ContainerFromIndex(index + 1) is ListBoxItem item){
                if(FindTextBox(item) is TextBox nextBox){
                    nextBox.Focus();
                    nextBox.CaretIndex = 0;
                }
            }
        }

        // 複数行テキストを貼り付けます。
        private void PasteMultiline(TextBox tb, string text){
            int index = lineList.ItemContainerGenerator.IndexFromContainer(lineList.ContainerFromElement(tb));
            if(index < 0) return;

            text = text.Replace("\r\n", "\n");
            string[] parts = text.Split('\n');

            int caret = tb.CaretIndex;
            if(DataContext is TextEditorViewModel vm){
                vm.LineService.PasteLines(lines, index, caret, text);
                for(int i = 1; i < parts.Length; i++){
                    lines[index + i].PropertyChanged += OnLineChanged;
                }
                Renumber();
                ScheduleVmUpdate();
            }

            lineList.UpdateLayout();
            if(lineList.ItemContainerGenerator.ContainerFromIndex(index + parts.Length - 1) is ListBoxItem item){
                if(FindTextBox(item) is TextBox nextBox){
                    nextBox.Focus();
                    nextBox.CaretIndex = parts[^1].Length;
                }
            }
        }

        // 指定した親要素から TextBox を再帰的に検索します。
        private static TextBox? FindTextBox(DependencyObject parent){
            for(int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++){
                DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if(child is TextBox tb) return tb;
                TextBox? result = FindTextBox(child);
                if(result != null) return result;
            }
            return null;
        }
    }
}
