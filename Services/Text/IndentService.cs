using System;
using System.Windows.Controls;
using Utils;

namespace Services{
    public static class IndentService{
        // 選択された行をインデントまたはインデント解除します。
        // Tab入力はEditorSettings.IndentStringで定義されたスペースに置き換えられます。
        public static void ModifySelection(TextBox editor, bool indent){
            string text = editor.Text.Replace("\r\n", "\n");
            string[] lines = text.Split('\n');
            int startLine = editor.GetLineIndexFromCharacterIndex(editor.SelectionStart);
            int endLine = editor.GetLineIndexFromCharacterIndex(editor.SelectionStart + editor.SelectionLength);

            int delta = 0;
            // Define the indent string here or reference your settings class appropriately.
            string indentStr = " ".Repeat(4); // 4 spaces as default, replace with your preferred indent or settings reference

            for(int i = startLine; i <= endLine && i < lines.Length; i++){
                string line = lines[i];
                if(indent){
                    line = indentStr + line.TrimStart('\t');
                    delta += indentStr.Length;
                }else if(line.StartsWith(indentStr)){
                    line = line.Substring(indentStr.Length);
                    delta -= indentStr.Length;
                }
                lines[i] = line;
            }

            editor.Text = string.Join(Environment.NewLine, lines);
            editor.SelectionStart = editor.GetCharacterIndexFromLineIndex(startLine);
            editor.SelectionLength = Math.Max(0, (editor.GetCharacterIndexFromLineIndex(endLine) + lines[endLine].Length) - editor.SelectionStart + delta);
        }
    }
}
