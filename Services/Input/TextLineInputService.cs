namespace Services;

using System.Collections.Generic;
using Controls;

public class TextLineInputService : ITextLineInputService
{
    public void InsertNewLine(IList<TextLine> lines, int index, int caretIndex)
    {
        if(index < 0 || index >= lines.Count) return;
        TextLine line = lines[index];
        string before = line.Text.Substring(0, System.Math.Min(caretIndex, line.Text.Length));
        string after = line.Text.Substring(System.Math.Min(caretIndex, line.Text.Length));
        line.Text = before;
        TextLine newLine = new TextLine { Text = after };
        lines.Insert(index + 1, newLine);
    }

    public void PasteLines(IList<TextLine> lines, int index, int caretIndex, string text)
    {
        if(index < 0 || index >= lines.Count) return;
        text = text.Replace("\r\n", "\n");
        string[] parts = text.Split('\n');
        TextLine line = lines[index];
        string before = line.Text.Substring(0, System.Math.Min(caretIndex, line.Text.Length));
        string after = line.Text.Substring(System.Math.Min(caretIndex, line.Text.Length));
        line.Text = before + parts[0];
        for(int i = 1; i < parts.Length; i++)
        {
            TextLine nl = new TextLine { Text = parts[i] };
            lines.Insert(index + i, nl);
        }
        lines[index + parts.Length - 1].Text += after;
    }
}
