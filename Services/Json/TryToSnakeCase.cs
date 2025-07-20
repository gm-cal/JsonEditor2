using System.Text;

namespace Services{
    public partial class JsonService : IJsonService{
        // --- スネークケース変換
        public bool TryToSnakeCase(string input, int indentWidth, out string converted, out string error){
            return TryRenameProperties(input, indentWidth, ToSnakeCase, out converted, out error);
        }
        private static string ToSnakeCase(string name){
            if(string.IsNullOrEmpty(name)) return name;

            int splitIndex = name.IndexOfAny(new char[]{' ', '\t'});
            string head = splitIndex >= 0 ? name.Substring(0, splitIndex) : name;
            string tail = splitIndex >= 0 ? name.Substring(splitIndex) : string.Empty;

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < head.Length; i++){
                char c = head[i];
                if(char.IsUpper(c)){
                    if(i > 0) sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }else{
                    sb.Append(c);
                }
            }
            sb.Append(tail);
            return sb.ToString();
        }
    }
}
