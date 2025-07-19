using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;

namespace Controls{
    public partial class TextEdit : UserControl{
        public TextEdit(){
            InitializeComponent();
            Editor.PreviewKeyDown += OnPreviewKeyDown;
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
            string text = Editor.Text.Replace("\r\n", "\n");
            string[] lines = text.Split('\n');
            int startLine = Editor.GetLineIndexFromCharacterIndex(Editor.SelectionStart);
            int endLine = Editor.GetLineIndexFromCharacterIndex(Editor.SelectionStart + Editor.SelectionLength);

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

            Editor.Text = string.Join(System.Environment.NewLine, lines);
            Editor.SelectionStart = Editor.GetCharacterIndexFromLineIndex(startLine);
            Editor.SelectionLength = System.Math.Max(0, (Editor.GetCharacterIndexFromLineIndex(endLine) + lines[endLine].Length) - Editor.SelectionStart + delta);
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
