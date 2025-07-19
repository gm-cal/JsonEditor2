using System.IO;

namespace Services{
    public partial class FileService : IFileService{
        // --- ファイル読み込み
        public string Load(string path){
            return File.ReadAllText(path);
        }
    }
}
