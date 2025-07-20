using System.Collections;
using System.Collections.Generic;
using Controls;

namespace Services{
    public static class SelectionService{
        public static (IList<TextLine> Lines, int StartLine, int EndLine) GetSelectedLineRange(IList<TextLine> lines, IList selectedItems, int selectedIndex){
            if(selectedItems.Count == 0)
                return (lines, selectedIndex, selectedIndex);

            int start = int.MaxValue;
            int end = int.MinValue;
            foreach(var item in selectedItems){
                if(item is TextLine line){
                    int idx = lines.IndexOf(line);
                    if(idx < start) start = idx;
                    if(idx > end) end = idx;
                }
            }
            if(start == int.MaxValue) start = selectedIndex;
            if(end == int.MinValue) end = selectedIndex;
            if(start > end) (start, end) = (end, start);
            return (lines, start, end);
        }
    }
}
