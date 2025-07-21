using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Services{
    // JSON操作に関するサービスを提供します。
    public partial class JsonService : IJsonService{
        // --- タブ区切りテキストをJSON形式に変換します。
        // input        タブ区切りテキスト
        // indentWidth  インデント幅
        // json         変換後のJSON文字列
        // error        エラーメッセージ
        // 戻り値       変換に成功した場合はtrue、失敗した場合はfalse
        public bool TryConvertTabToJson(string input, int indentWidth, out string json, out string error){
            error = string.Empty;
            json = string.Empty;
            try{
                string[] lines = input.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);
                Regex jsonLine = new("^\"\\s*([^\"\\s]+)\\s*\"\\s*:\\s*\"(.*)\"$", RegexOptions.Compiled);
                bool allJson = lines.All(l => jsonLine.IsMatch(l.Trim()));

                List<string> resultLines = new();
                if(allJson){
                    foreach(string line in lines){
                        Match m = jsonLine.Match(line.Trim());
                        resultLines.Add($"{m.Groups[1].Value}\t{m.Groups[2].Value}");
                    }
                    json = string.Join("\n", resultLines);
                }else{
                    foreach(string line in lines){
                        string trimmed = line.Trim();
                        Match m = jsonLine.Match(trimmed);
                        if(m.Success){
                            resultLines.Add(line);
                        }else{
                            string[] parts = trimmed.Split(new char[]{'\t',' '}, 2, StringSplitOptions.RemoveEmptyEntries);
                            string key = parts.Length > 0 ? parts[0] : string.Empty;
                            string value = parts.Length > 1 ? parts[1] : string.Empty;
                            string indent = new(' ', indentWidth);
                            resultLines.Add($"{indent}\"{key}\": \"{value}\"");
                        }
                    }
                    json = string.Join(",\n", resultLines);
                }

                return true;
            }catch(Exception ex){
                error = ex.Message;
                return false;
            }
        }
    }
}
