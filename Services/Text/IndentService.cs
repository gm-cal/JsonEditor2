using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Utils;

namespace Services{
    public static class IndentService{
        // 選択範囲の項目名と値の間にインデント/逆インデントを適用します。
        // TabはEditorSettings.IndentStringで定義されたスペースに置き換えられます。
        public static void ModifySelection(TextBox editor, bool indent){
            string text = editor.Text.Replace("\r\n", "\n");
            string[] lines = text.Split('\n');
            int startLine = editor.GetLineIndexFromCharacterIndex(editor.SelectionStart);
            int endLine = editor.GetLineIndexFromCharacterIndex(editor.SelectionStart + editor.SelectionLength);

            string indentStr = EditorSettings.IndentString;
            List<int> measures = new();
            List<int> keyEnds = new();

            for(int i = startLine; i <= endLine && i < lines.Length; i++){
                string line = lines[i];
                int keyEnd = GetKeyEnd(line);
                int measure = GetMeasure(line, keyEnd);
                keyEnds.Add(keyEnd);
                measures.Add(measure);
            }

            int delta = 0;
            int minMeasure = measures.Count > 0 ? measures.Min() : 0;

            for(int i = 0; i < measures.Count; i++){
                int lineIndex = startLine + i;
                string line = lines[lineIndex];

                if(indent){
                    if(measures[i] == minMeasure){
                        line = line.Insert(keyEnds[i], indentStr);
                        delta += indentStr.Length;
                    }
                }else{
                    if(line.Length >= keyEnds[i] + indentStr.Length && line.Substring(keyEnds[i], indentStr.Length) == indentStr){
                        line = line.Remove(keyEnds[i], indentStr.Length);
                        delta -= indentStr.Length;
                    }
                }
                lines[lineIndex] = line;
            }

            editor.Text = string.Join(Environment.NewLine, lines);
            editor.SelectionStart = editor.GetCharacterIndexFromLineIndex(startLine);
            editor.SelectionLength = Math.Max(0, (editor.GetCharacterIndexFromLineIndex(endLine) + lines[endLine].Length) - editor.SelectionStart + delta);
        }

        private static int GetKeyEnd(string line){
            int index = 0;
            while(index < line.Length && char.IsWhiteSpace(line[index])) index++;
            while(index < line.Length && !char.IsWhiteSpace(line[index]) && line[index] != ':') index++;
            return index;
        }

        private static int GetMeasure(string line, int keyEnd){
            int colon = line.IndexOf(':', keyEnd);
            int valueStart = keyEnd;
            while(valueStart < line.Length && char.IsWhiteSpace(line[valueStart])) valueStart++;
            if(colon >= 0 && colon < valueStart) return colon;
            return valueStart;
        }
    }
}
