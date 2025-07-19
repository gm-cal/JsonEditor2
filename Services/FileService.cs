using System.IO;

namespace JsonEditor2.Services{
    public class FileService : IFileService{
        public string Load(string path){
            return File.ReadAllText(path);
        }

        public void Save(string path, string content){
            File.WriteAllText(path, content);
        }
    }
}
