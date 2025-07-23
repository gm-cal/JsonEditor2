namespace Services{
    public interface ITextService{
        bool TryConvertTabToJson(string input, int indentWidth, out string json, out string error);
        bool TryFormatJson(string input, int indentWidth, out string formatted, out string error);
        bool TryToUpperCamel(string input, int indentWidth, out string converted, out string error);
        bool TryToSnakeCase(string input, int indentWidth, out string converted, out string error);
    }
}