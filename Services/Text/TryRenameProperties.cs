using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Services{
    public partial class TextService : ITextService{
        private static bool TryRenameProperties(string input, int indentWidth, Func<string, string> converter, out string result, out string error){
            error = string.Empty;
            result = string.Empty;
            try{
                JsonNode root = JsonNode.Parse(input)!;
                Rename(root, converter);
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
    }
}
