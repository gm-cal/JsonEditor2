using System.Text;

namespace Services{
    public partial class JsonService : IJsonService{
        // --- スネークケース変換
        public bool TryToSnakeCase(string input, int indentWidth, out string converted, out string error){
            return TryRenameProperties(input, indentWidth, ToSnakeCase, out converted, out error);
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