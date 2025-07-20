namespace Services{
    public partial class JsonService : IJsonService{
        private static bool IsStructureLine(string line){
            string trimmed = line.Trim();
            return trimmed == "{" || trimmed == "}" || trimmed == "[" || trimmed == "]";
        }
    }
}
