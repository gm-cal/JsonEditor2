using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Utils;

namespace Services{
    public partial class TextService : ITextService{
        // 選択範囲の項目名と値の間にインデント/逆インデントを適用します。
        // TabはEditorSettings.IndentStringで定義されたスペースに置き換えられます。
        public void ModifySelection(string[] input, bool indent, out string[] output){
            string indentStr = EditorSettings.IndentString;
            List<int> measures = new List<int>();   // 各行の項目名と値の間のスペースの長さ
            List<int> keyEnds  = new List<int>();   // 各行の項目名の終端位置

            foreach(string line in input){
                if(string.IsNullOrWhiteSpace(line)) continue; // 空行は無視
                int keyEnd = GetKeyEnd(line);
                int measure = GetMeasure(line, keyEnd);
                keyEnds.Add(keyEnd);
                measures.Add(measure);
            }

            int delta = 0;  // インデントによる文字数の変化量
            int minMeasure = measures.Count > 0 ? measures.Min() : 0;

            // インデント/逆インデントを適用
            for(int i = 0; i < measures.Count; i++){
                string line = input[i];

                if(indent){
                    if(measures[i] == minMeasure){
                        line = line.Insert(keyEnds[i], indentStr);
                        delta += indentStr.Length;
                    }
                }else{
                    if(line.Length >= keyEnds[i] + indentStr.Length && line.Substring(keyEnds[i], indentStr.Length) == indentStr){
                        line = line.Remove(keyEnds[i], indentStr.Length);
                        delta -= indentStr.Length;
                    }
                }
                input[i] = line;
            }
            output = new string[input.Length];
            for(int i = 0; i < input.Length; i++){
                string line = input[i];
                if(measures[i] > 0){
                    int keyEnd = keyEnds[i];
                    int measure = measures[i] + delta;
                    if(measure < 0) measure = 0; // 負の値はゼロにする
                    if(measure > keyEnd) measure = keyEnd; // キーの終端を超えないようにする
                    output[i] = line.Substring(0, keyEnd) + new string(' ', measure - keyEnd) + line.Substring(keyEnd);
                }else{
                    output[i] = line; // 空行はそのまま
                }
            }
        }

        private static int GetKeyEnd(string line){
            int index = 0;
            while(index < line.Length && char.IsWhiteSpace(line[index])) index++;
            while(index < line.Length && !char.IsWhiteSpace(line[index]) && line[index] != ':') index++;
            return index;
        }

        private static int GetMeasure(string line, int keyEnd){
            int colon = line.IndexOf(':', keyEnd);
            int valueStart = keyEnd;
            while(valueStart < line.Length && char.IsWhiteSpace(line[valueStart])) valueStart++;
            if(colon >= 0 && colon < valueStart) return colon;
            return valueStart;
        }
    }
}
