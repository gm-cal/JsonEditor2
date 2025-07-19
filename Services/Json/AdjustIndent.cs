namespace Services{
    public partial class JsonService : IJsonService{
        // --- JSON文字列のインデント幅を調整します。
        // json         インデント調整対象のJSON文字列
        // indentWidth  インデント幅
        // 戻り値       インデント幅を調整したJSON文字列
        private static string AdjustIndent(string json, int indentWidth){
            string indent = new string(' ', indentWidth);
            string[] lines = json.Replace("\r\n", "\n").Split('\n');
            for(int i = 0; i < lines.Length; i++){
                lines[i] = lines[i].Replace("    ", indent);
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
