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
                Rename(root);
                JsonSerializerOptions options = new JsonSerializerOptions{ WriteIndented = true };
                string raw = JsonSerializer.Serialize(root, options);
                result = AdjustIndent(raw, indentWidth);
                return true;
            }catch(JsonException ex){
                error = $"Line {ex.LineNumber}, Position {ex.BytePositionInLine}: {ex.Message}";
                return false;
            }catch(Exception ex){
                error = ex.Message;
                return false;
            }
        }
        private static void Rename(JsonNode? node){
            if(node is JsonObject obj){
                List<KeyValuePair<string, JsonNode?>> props = obj.ToList();
                foreach(KeyValuePair<string, JsonNode?> p in props){
                    obj.Remove(p.Key);
                }
                foreach(KeyValuePair<string, JsonNode?> p in props){
                    string newName = ToSnakeCase(p.Key);
                    Rename(p.Value);
                    obj[newName] = p.Value;
                }
            }else if(node is JsonArray arr){
                foreach(JsonNode? child in arr){
                    Rename(child);
                }
            }
        }
    }
}
