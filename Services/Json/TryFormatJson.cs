using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Services{
    // JSON操作に関するサービスを提供します。
    public partial class JsonService : IJsonService{
        // --- JSON文字列を指定したインデント幅で整形します。
        // input        整形対象のJSON文字列
        // indentWidth  インデント幅
        // formatted    整形後のJSON文字列
        // error        エラーメッセージ
        // 戻り値       整形に成功した場合はtrue、失敗した場合はfalse
        public bool TryFormatJson(string input, int indentWidth, out string formatted, out string error){
            error = string.Empty;
            formatted = string.Empty;
            try{
                using JsonDocument doc = JsonDocument.Parse(input);
                JsonSerializerOptions options = new JsonSerializerOptions{ WriteIndented = true };
                string raw = JsonSerializer.Serialize(doc.RootElement, options);
                formatted = AdjustIndent(raw, indentWidth);
                return true;
            }catch(JsonException ex){
                error = $"Line {ex.LineNumber}, Position {ex.BytePositionInLine}: {ex.Message}";
                return false;
            }catch(Exception ex){
                error = ex.Message;
                return false;
            }
        }
    }
}
