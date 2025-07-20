using System;
using System.Collections.Generic;
using Services;

namespace Controls{
    public partial class TextEdit{
        public void IndentSelection(){
            ModifySelection(true);
        }

        public void UnindentSelection(){
            ModifySelection(false);
        }

        private void ModifySelection(bool indent){
            var (currentLines, start, end) = SelectionService.GetSelectedLineRange(lines, lineList.SelectedItems, lineList.SelectedIndex);
            IndentService.ModifySelection(currentLines, start, end, indent);
            Renumber();
            ScheduleVmUpdate();
        }

        private void Renumber(){
            for(int i = 0; i < lines.Count; i++)
                lines[i].LineNumber = i + 1;
        }

        public (IList<TextLine> Lines, int StartLine, int EndLine) GetSelectedLineRange(){
            return SelectionService.GetSelectedLineRange(lines, lineList.SelectedItems, lineList.SelectedIndex);
        }
    }
}
