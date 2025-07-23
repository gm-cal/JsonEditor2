namespace Services;

using System.Collections.Generic;
using Controls;

public interface ITextLineInputService
{
    void InsertNewLine(IList<TextLine> lines, int index, int caretIndex);
    void PasteLines(IList<TextLine> lines, int index, int caretIndex, string text);
}
