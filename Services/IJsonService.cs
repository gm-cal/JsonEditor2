namespace JsonEditor2.Services{
    public interface IJsonService{
        bool TryConvertTabToJson(string input, int indentWidth, out string json, out string error);
        bool TryFormatJson(string input, int indentWidth, out string formatted, out string error);
    }
}
