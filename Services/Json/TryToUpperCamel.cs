using System.Text;

namespace Services{
    public partial class JsonService : IJsonService{
        // --- アッパーキャメルケース変換
        public bool TryToUpperCamel(string input, int indentWidth, out string converted, out string error){
            return TryRenameProperties(input, indentWidth, ToUpperCamel, out converted, out error);
        }

        private static string ToUpperCamel(string name){
            if(string.IsNullOrEmpty(name)) return name;
            string[] parts = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length == 0) return name;

            StringBuilder sb = new StringBuilder();
            sb.Append(parts[0].ToLowerInvariant());
            for(int i = 1; i < parts.Length; i++){
                string part = parts[i];
                if(part.Length > 0){
                    sb.Append(char.ToUpperInvariant(part[0]));
                    if(part.Length > 1) sb.Append(part.Substring(1).ToLowerInvariant());
                }
            }
            return sb.ToString();
        }
    }
}
