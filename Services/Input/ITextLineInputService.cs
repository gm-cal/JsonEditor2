using Controls;

namespace Services{
    // テキスト行入力サービスのインターフェース
    // テキスト行の挿入やペーストなどの操作を提供します。
    public interface ITextLineInputService{
        void InsertNewLine(IList<TextLine> lines, int index, int caretIndex);
        void PasteLines(IList<TextLine> lines, int index, int caretIndex, string text);
    }
}