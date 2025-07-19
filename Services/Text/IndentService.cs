using System;
using System.Windows.Controls;

namespace Services{
    public static class IndentService{
        // Modify selected lines by indenting or unindenting.
        // Tab input is replaced with spaces defined by EditorSettings.IndentString.
        public static void ModifySelection(TextBox editor, bool indent){
            string text = editor.Text.Replace("\r\n", "\n");
            string[] lines = text.Split('\n');
            int startLine = editor.GetLineIndexFromCharacterIndex(editor.SelectionStart);
            int endLine = editor.GetLineIndexFromCharacterIndex(editor.SelectionStart + editor.SelectionLength);

            int delta = 0;
            string indentStr = EditorSettings.IndentString;

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
