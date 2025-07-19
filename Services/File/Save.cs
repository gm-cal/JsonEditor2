using System.IO;

namespace Services{
    public partial class FileService : IFileService{
        // --- ファイル保存
        public void Save(string path, string content){
            File.WriteAllText(path, content);
        }
    }
}