using System;
using System.Collections.Generic;
using Controls;
using Utils;

namespace Services{
    public static class IndentService{
        // 選択された行をインデントまたはインデント解除します。
        // Tab入力はEditorSettings.IndentStringで定義されたスペースに置き換えられます。
        public static void Indent(IList<TextLine> lines, int startLine, int endLine) =>
            ModifySelection(lines, startLine, endLine, true);

        public static void Unindent(IList<TextLine> lines, int startLine, int endLine) =>
            ModifySelection(lines, startLine, endLine, false);

        public static void ModifySelection(IList<TextLine> lines, int startLine, int endLine, bool indent){
            string indentStr = EditorSettings.IndentString;

            for(int i = startLine; i <= endLine && i < lines.Count; i++){
                string line = lines[i].Text;
                int colonIndex = line.IndexOf(':');
                if(colonIndex >= 0){
                    string before = line.Substring(0, colonIndex);
                    string after = line.Substring(colonIndex);
                    if(indent){
                        before = indentStr + before;
                    }else if(before.StartsWith(indentStr)){
                        before = before.Substring(indentStr.Length);
                    }
                    line = before + after;
                }else{
                    if(indent){
                        line = indentStr + line.TrimStart('\t');
                    }else if(line.StartsWith(indentStr)){
                        line = line.Substring(indentStr.Length);
                    }
                }
                lines[i].Text = line;
            }
        }
    }
}
