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
            var (currentLines, start, end) = GetSelectedLineRange();
            IndentService.ModifySelection(currentLines, start, end, indent);
            Renumber();
            ScheduleVmUpdate();
        }

        private void Renumber(){
            for(int i = 0; i < lines.Count; i++)
                lines[i].LineNumber = i + 1;
        }

        public (IList<TextLine> Lines, int StartLine, int EndLine) GetSelectedLineRange(){
            if(lineList.SelectedItems.Count == 0)
                return (lines, lineList.SelectedIndex, lineList.SelectedIndex);

            int start = lineList.Items.IndexOf(lineList.SelectedItems[0]);
            int end = lineList.Items.IndexOf(lineList.SelectedItems[^1]);
            if(start > end) (start, end) = (end, start);
            return (lines, start, end);
        }
    }
}
