using System;
using System.Collections.Generic;
using System.Text.Json;
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
                string[] lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Regex jsonFormatRegex = new Regex("^\"\\s*([^\"\\s]+)\\s*\"\\s*:\\s*\"(.*)\"$");
                List<string> resultLines = new List<string>();

                bool allAreJsonFormat = lines.All(line => jsonFormatRegex.IsMatch(line.Trim()));

                if (allAreJsonFormat) {
                    // すべて変換後の書式なら変換前の書式に戻す
                    foreach (string line in lines) {
                        string trimmed = line.Trim();
                        Match match = jsonFormatRegex.Match(trimmed);
                        string key = match.Groups[1].Value;
                        string value = match.Groups[2].Value;
                        resultLines.Add($"{key}\t{value}");
                    }
                    json = string.Join("\n", resultLines);
                } else {
                    // 変換前の行のみ変換後の書式に変換、変換後の行はそのまま
                    foreach (string line in lines) {
                        string trimmed = line.Trim();
                        Match match = jsonFormatRegex.Match(trimmed);
                        if (match.Success) {
                            // 変換後の書式はそのまま
                            resultLines.Add(line);
                        } else {
                            // 変換前の書式のみ変換
                            string[] parts = trimmed.Split(new char[]{'\t', ' '}, 2, StringSplitOptions.RemoveEmptyEntries);
                            string key = parts.Length > 0 ? parts[0] : "";
                            string value = parts.Length > 1 ? parts[1] : "";
                            string indent = new string(' ', indentWidth);
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
