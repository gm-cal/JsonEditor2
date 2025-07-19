using System;
using System.Collections.Generic;
using System.Text.Json;

namespace JsonEditor2.Services{
    public class JsonService : IJsonService{
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

        private static string AdjustIndent(string json, int indentWidth){
            string indent = new string(' ', indentWidth);
            string[] lines = json.Split('\n');
            for(int i = 0; i < lines.Length; i++){
                lines[i] = lines[i].Replace("    ", indent);
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
