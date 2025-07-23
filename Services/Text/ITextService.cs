using Controls;

namespace Services{
    public interface ITextService{
        bool TryToUpperCamel(string input, int indentWidth, out string converted, out string error);
        bool TryToSnakeCase(string input, int indentWidth, out string converted, out string error);
        void Indent(IList<TextLine> lines, int startLine, int endLine);
        void Unindent(IList<TextLine> lines, int startLine, int endLine);
    }
}