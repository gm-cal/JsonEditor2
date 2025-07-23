using System;

namespace Services{
    public partial class TextService : ITextService{
        private static string AdjustIndent(string json, int indentWidth){
            string indent = new string(' ', indentWidth);
            string[] lines = json.Replace("\r\n", "\n").Split('\n');
            for(int i = 0; i < lines.Length; i++){
                lines[i] = lines[i].Replace("    ", indent);
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
