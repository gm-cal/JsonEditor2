using System.Collections.Generic;
using Controls;

namespace Services{
    public partial class TextService : ITextService{
        public void Indent(IList<TextLine> lines, int startLine, int endLine){
            IndentService.Indent(lines, startLine, endLine);
        }

        public void Unindent(IList<TextLine> lines, int startLine, int endLine){
            IndentService.Unindent(lines, startLine, endLine);
        }
    }
}
