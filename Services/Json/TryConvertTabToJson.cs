using System;
using System.Collections.Generic;
using System.Text.Json;

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
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string[] lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string line in lines){
                    string[] parts = line.Split('\t');
                    if(parts.Length < 2){
                        error = $"Invalid line: {line}";
                        return false;
                    }
                    dict[parts[0]] = parts[1];
                }
                JsonSerializerOptions options = new JsonSerializerOptions{ WriteIndented = true };
                string raw = JsonSerializer.Serialize(dict, options);
                json = AdjustIndent(raw, indentWidth);
                return true;
            }catch(Exception ex){
                error = ex.Message;
                return false;
            }
        }
    }
}
