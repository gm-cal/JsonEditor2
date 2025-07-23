using System.Collections.Generic;
using Controls;

namespace Services{
    public partial class TextLineInputService : ITextLineInputService{
        public void InsertNewLine(IList<TextLine> lines, int index, int caretIndex){
            if(index < 0 || index >= lines.Count) return;
            TextLine line = lines[index];
            string before = line.Text.Substring(0, System.Math.Min(caretIndex, line.Text.Length));
            string after = line.Text.Substring(System.Math.Min(caretIndex, line.Text.Length));
            line.Text = before;
            TextLine newLine = new TextLine { Text = after };
            lines.Insert(index + 1, newLine);
        }
    }
}
