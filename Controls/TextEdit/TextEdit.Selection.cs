using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls{
    public partial class TextEdit{
        private bool isDragSelecting = false;
        private int dragStartIndex = -1;

        // イベントハンドラを追加して、ドラッグ選択とコピー機能を実装します。
        private void OnEditorPreviewKeyDown(object sender, KeyEventArgs e){
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control){
                if(e.Key == Key.A){
                    lineList.SelectAll();
                    e.Handled = true;
                }else if(e.Key == Key.C){
                    CopySelection();
                    e.Handled = true;
                }
            }
        }

        // マウス左ボタンが押されたときの処理
        private void OnEditorPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e){
            if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                return;
            if(GetParentTextBox(e.OriginalSource as DependencyObject) != null)
                return;
            int index = IndexFromPoint(e.GetPosition(lineList));
            if(index >= 0){
                dragStartIndex = index;
                isDragSelecting = true;
                lineList.SelectedItems.Clear();
                lineList.SelectedItems.Add(lineList.Items[index]);
                lineList.CaptureMouse();
                e.Handled = true;
            }
        }

        // マウスが動いたときの処理
        private void OnEditorMouseMove(object sender, MouseEventArgs e){
            if(isDragSelecting){
                int index = IndexFromPoint(e.GetPosition(lineList));
                if(index >= 0){
                    lineList.SelectedItems.Clear();
                    int start = dragStartIndex < index ? dragStartIndex : index;
                    int end = dragStartIndex > index ? dragStartIndex : index;
                    for(int i = start; i <= end; i++)
                        lineList.SelectedItems.Add(lineList.Items[i]);
                }
            }
        }

        // マウス左ボタンが離されたときの処理
        private void OnEditorPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e){
            if(isDragSelecting){
                isDragSelecting = false;
                lineList.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        // マウスが離れたときの処理
        private int IndexFromPoint(Point point){
            DependencyObject? element = lineList.InputHitTest(point) as DependencyObject;
            while(element != null && element is not ListBoxItem)
                element = VisualTreeHelper.GetParent(element);
            if(element is ListBoxItem item)
                return lineList.ItemContainerGenerator.IndexFromContainer(item);
            return -1;
        }

        // 選択された行の範囲を取得します。
        private static TextBox? GetParentTextBox(DependencyObject? obj){
            while(obj != null){
                if(obj is TextBox tb) return tb;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

        // 選択された行の範囲を取得します。
        private void CopySelection(){
            (IList<TextLine> lines, int start, int end) = GetSelectedLineRange();
            StringBuilder sb = new();
            for(int i = start; i <= end; i++)
                sb.AppendLine(lines[i].Text);
            Clipboard.SetText(sb.ToString().TrimEnd('\r', '\n'));
        }
    }
}

