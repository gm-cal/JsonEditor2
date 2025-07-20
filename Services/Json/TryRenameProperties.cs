using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using System.Windows.Input;

namespace Services{
    public partial class JsonService : IJsonService{
        private static bool TryRenameProperties(string input, int indentWidth, Func<string, string> converter, out string result, out string error){
            error = string.Empty;
            result = string.Empty;
            try{
                JsonNode root = JsonNode.Parse(input)!;
                Rename(root, converter);

                JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
                string raw = JsonSerializer.Serialize(root, options);

                // 出力後の整形
                string[] originalLines = input.Split('\n');
                string[] convertedLines = AdjustIndent(raw, indentWidth).Split('\n');

                // 構造だけの行を変換前から差し戻す
                for(int i = 0; i < convertedLines.Length && i < originalLines.Length; i++){
                    if(IsStructureLine(convertedLines[i])){
                        convertedLines[i] = originalLines[i];
                    }
                }

                result = string.Join("\n", convertedLines);
                return true;
            }catch(JsonException){
                result = RenamePlainText(input, converter);
                return true;
            }catch(Exception ex){
                error = ex.Message;
                return false;
            }
        }

        private static void Rename(JsonNode? node, Func<string, string> converter){
            if(node is JsonObject obj){
                List<KeyValuePair<string, JsonNode?>> props = obj.ToList();
                obj.Clear();
                foreach(KeyValuePair<string, JsonNode?> p in props){
                    Rename(p.Value, converter);
                    string newName = converter(p.Key);
                    obj[newName] = p.Value;
                }
            }else if(node is JsonArray arr){
                foreach(JsonNode? child in arr){
                    Rename(child, converter);
                }
            }
        }

        private static string RenamePlainText(string input, Func<string, string> converter){
            string[] lines = input.Replace("\r\n", "\n").Split('\n');
            for(int i = 0; i < lines.Length; i++){
                lines[i] = converter(lines[i]);
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
