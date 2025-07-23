// TextEdit コントロールのインデント操作を提供します。
// 選択範囲のインデント・アンインデント、行番号の再設定、
// 選択範囲取得などの機能を持ちます。
using System;
using System.Collections.Generic;
using Services;

namespace Controls{
    public partial class TextEdit{
        // 選択された行をインデントします。
        public void IndentSelection(){
            ApplyIndent(true);
        }

        // 選択された行をアンインデントします。
        public void UnindentSelection(){
            ApplyIndent(false);
        }

        // 選択された行のインデントまたはアンインデントを適用します。
        private void ApplyIndent(bool indent){
            (IList<TextLine> currentLines, int start, int end) = SelectionService.GetSelectedLineRange(lines, lineList.SelectedItems, lineList.SelectedIndex);
            if(indent)
                IndentService.Indent(currentLines, start, end);
            else
                IndentService.Unindent(currentLines, start, end);
            Renumber();
            ScheduleVmUpdate();
        }

        // 選択範囲の行番号を再設定します。
        private void Renumber(){
            for(int i = 0; i < lines.Count; i++)
                lines[i].LineNumber = i + 1;
        }

        // 選択範囲の行を取得します。
        public (IList<TextLine> Lines, int StartLine, int EndLine) GetSelectedLineRange(){
            return SelectionService.GetSelectedLineRange(lines, lineList.SelectedItems, lineList.SelectedIndex);
        }
    }
}
