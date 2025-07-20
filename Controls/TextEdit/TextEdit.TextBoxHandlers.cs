using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Controls{
    public partial class TextEdit{
        private void OnLineNumberMouseDown(object sender, MouseButtonEventArgs e){
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
                if(sender is FrameworkElement element && element.DataContext is TextLine line){
                    LineControlRequested?.Invoke(this, line.LineNumber);
                    e.Handled = true;
                }
            }
        }

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

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e){
            if(sender is TextBox tb && !tb.IsKeyboardFocusWithin){
                tb.Focus();
                e.Handled = true;
            }
        }

        private void InsertNewLine(TextBox tb){
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
            if(lineList.ItemContainerGenerator.ContainerFromIndex(index + 1) is ListBoxItem item){
                if(FindTextBox(item) is TextBox nextBox){
                    nextBox.Focus();
                    nextBox.CaretIndex = 0;
                }
            }
        }

        private void PasteMultiline(TextBox tb, string text){
            int index = lineList.ItemContainerGenerator.IndexFromContainer(lineList.ContainerFromElement(tb));
            if(index < 0) return;

            text = text.Replace("\r\n", "\n");
            string[] parts = text.Split('\n');

            TextLine line = lines[index];
            int caret = tb.CaretIndex;
            string before = line.Text.Substring(0, Math.Min(caret, line.Text.Length));
            string after = line.Text.Substring(Math.Min(caret, line.Text.Length));

            line.Text = before + parts[0];

            for(int i = 1; i < parts.Length; i++){
                TextLine nl = new TextLine { Text = parts[i] };
                nl.PropertyChanged += OnLineChanged;
                lines.Insert(index + i, nl);
            }

            lines[index + parts.Length - 1].Text += after;
            Renumber();
            ScheduleVmUpdate();

            lineList.UpdateLayout();
            if(lineList.ItemContainerGenerator.ContainerFromIndex(index + parts.Length - 1) is ListBoxItem item){
                if(FindTextBox(item) is TextBox nextBox){
                    nextBox.Focus();
                    nextBox.CaretIndex = parts[^1].Length;
                }
            }
        }

        private static TextBox? FindTextBox(DependencyObject parent){
            for(int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++){
                DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if(child is TextBox tb) return tb;
                var result = FindTextBox(child);
                if(result != null) return result;
            }
            return null;
        }
    }
}
