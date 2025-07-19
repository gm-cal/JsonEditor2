namespace Services{
    public partial class JsonService : IJsonService{
        // --- アッパーキャメルケース変換
        public bool TryToUpperCamel(string input, int indentWidth, out string converted, out string error){
            return TryRenameProperties(input, indentWidth, ToUpperCamel, out converted, out error);
        }

        private static string ToUpperCamel(string name){
            if(string.IsNullOrEmpty(name)) return name;
            if(name.Length == 1) return char.ToUpperInvariant(name[0]).ToString();
            return char.ToUpperInvariant(name[0]) + name.Substring(1);
        }
    }
}
