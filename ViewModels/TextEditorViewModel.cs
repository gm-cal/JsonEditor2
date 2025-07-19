using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JsonEditor2.Services;
using Utils;

namespace JsonEditor2.ViewModels{
    public class TextEditorViewModel : INotifyPropertyChanged{
        private readonly IFileService fileService;
        private readonly IJsonService jsonService;
        private string text = string.Empty;
        private string filePath = string.Empty;
        private string status = string.Empty;
        private int indentWidth = 4;
        private string title = string.Empty;
        private static int newFileCounter = 1;

        public TextEditorViewModel(IFileService fileService, IJsonService jsonService){
            this.fileService = fileService;
            this.jsonService = jsonService;
            ConvertCommand = new RelayCommand(_ => Convert());
            FormatCommand = new RelayCommand(_ => Format());
            OpenCommand = new RelayCommand(_ => Open());
            SaveCommand = new RelayCommand(_ => Save());
            Title = $"新規テキスト_{newFileCounter++}";
        }

        public string Text{
            get{ return text; }
            set{ text = value; OnPropertyChanged(); }
        }

        public string Status{
            get{ return status; }
            set{ status = value; OnPropertyChanged(); }
        }

        public ICommand ConvertCommand{ get; }
        public ICommand FormatCommand{ get; }
        public ICommand OpenCommand{ get; }
        public ICommand SaveCommand{ get; }

        public int IndentWidth{
            get{ return indentWidth; }
            set{ indentWidth = value; OnPropertyChanged(); }
        }

        public string Title{
            get{ return title; }
            private set{ title = value; OnPropertyChanged(); }
        }

        public string FilePath{
            get{ return filePath; }
            set{
                filePath = value;
                if(string.IsNullOrEmpty(filePath)){
                    Title = $"新規テキスト_{newFileCounter++}";
                }else{
                    Title = Path.GetFileName(filePath);
                }
                OnPropertyChanged();
            }
        }

        private void Convert(){
            if(jsonService.TryConvertTabToJson(Text, IndentWidth, out string json, out string error)){
                Text = json;
                Status = "変換しました";
            }else{
                Status = error;
            }
        }

        private void Format(){
            if(jsonService.TryFormatJson(Text, IndentWidth, out string formatted, out string error)){
                Text = formatted;
                Status = "整形しました";
            }else{
                Status = error;
            }
        }

        private void Open(){
            if(File.Exists(FilePath)){
                Text = fileService.Load(FilePath);
                Status = $"{Path.GetFileName(FilePath)} を読み込みました";
            }else{
                Status = "ファイルが見つかりません";
            }
        }

        private void Save(){
            fileService.Save(FilePath, Text);
            Status = $"{Path.GetFileName(FilePath)} を保存しました";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
