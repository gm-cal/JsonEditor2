using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;

namespace Controls{
    public partial class TextEdit : UserControl{
        private TextBox editorControl => Editor;

        public TextEdit(){
            InitializeComponent();
            editorControl.PreviewKeyDown += OnPreviewKeyDown;
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
            }
        }

        private void ModifySelection(bool indent){
            string text = editorControl.Text.Replace("\r\n", "\n");
            string[] lines = text.Split('\n');
            int startLine = editorControl.GetLineIndexFromCharacterIndex(editorControl.SelectionStart);
            int endLine = editorControl.GetLineIndexFromCharacterIndex(editorControl.SelectionStart + editorControl.SelectionLength);

            int delta = 0;
            string indentStr = EditorSettings.IndentString;

            for(int i = startLine; i <= endLine && i < lines.Length; i++){
                string line = lines[i];
                int colon = line.IndexOf(':');
                if(colon <= 0) continue;

                if(indent){
                    line = line.Insert(colon, indentStr);
                    delta += indentStr.Length;
                }else if(colon >= indentStr.Length && line.Substring(colon - indentStr.Length, indentStr.Length) == indentStr){
                    line = line.Remove(colon - indentStr.Length, indentStr.Length);
                    delta -= indentStr.Length;
                }
                lines[i] = line;
            }

            editorControl.Text = string.Join(Environment.NewLine, lines);
            editorControl.SelectionStart = editorControl.GetCharacterIndexFromLineIndex(startLine);
            editorControl.SelectionLength = Math.Max(0, (editorControl.GetCharacterIndexFromLineIndex(endLine) + lines[endLine].Length) - editorControl.SelectionStart + delta);
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
    }
}
