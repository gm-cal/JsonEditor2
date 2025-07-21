using System;
using System.Text.Json;
using System.Text.Json.Nodes;

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
            try{
                JsonNode node = JsonNode.Parse(input)!;
                JsonSerializerOptions options = new JsonSerializerOptions{ WriteIndented = true };
                string raw = JsonSerializer.Serialize(node, options);
                formatted = AdjustIndent(raw, indentWidth);
                error = string.Empty;
                return true;

            }catch(JsonException ex){
                error = $"Line {ex.LineNumber}, Position {ex.BytePositionInLine}: {ex.Message}";
                formatted = input;
                return false;
            }catch(Exception ex){
                error = ex.Message;
                formatted = input;
                return false;
            }
        }
    }
}
