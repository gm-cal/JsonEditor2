namespace JsonEditor2.Services{
    public interface IFileService{
        string Load(string path);
        void Save(string path, string content);
    }
}
