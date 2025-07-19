using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;

namespace Services{
    public partial class JsonService : IJsonService{
        public bool TryToUpperCamel(string input, int indentWidth, out string converted, out string error){
            return TryRenameProperties(input, indentWidth, ToUpperCamel, out converted, out error);
        }

        public bool TryToSnakeCase(string input, int indentWidth, out string converted, out string error){
            return TryRenameProperties(input, indentWidth, ToSnakeCase, out converted, out error);
        }

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

            void Rename(JsonNode? node){
                if(node is JsonObject obj){
                    var props = obj.ToList();
                    foreach(var p in props){
                        obj.Remove(p.Key);
                    }
                    foreach(var p in props){
                        string newName = converter(p.Key);
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

        private static string ToUpperCamel(string name){
            if(string.IsNullOrEmpty(name)) return name;
            if(name.Length == 1) return char.ToUpperInvariant(name[0]).ToString();
            return char.ToUpperInvariant(name[0]) + name.Substring(1);
        }

        private static string ToSnakeCase(string name){
            if(string.IsNullOrEmpty(name)) return name;
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < name.Length; i++){
                char c = name[i];
                if(char.IsUpper(c)){
                    if(i > 0) sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }else{
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
