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

        public TextEditorViewModel(IFileService fileService, IJsonService jsonService){
            this.fileService = fileService;
            this.jsonService = jsonService;
            ConvertCommand = new RelayCommand(_ => Convert());
            FormatCommand = new RelayCommand(_ => Format());
            OpenCommand = new RelayCommand(_ => Open());
            SaveCommand = new RelayCommand(_ => Save());
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

        public string FilePath{
            get{ return filePath; }
            set{ filePath = value; OnPropertyChanged(); }
        }

        private void Convert(){
            if(jsonService.TryConvertTabToJson(Text, IndentWidth, out string json, out string error)){
                Text = json;
                Status = "Converted";
            }else{
                Status = error;
            }
        }

        private void Format(){
            if(jsonService.TryFormatJson(Text, IndentWidth, out string formatted, out string error)){
                Text = formatted;
                Status = "Formatted";
            }else{
                Status = error;
            }
        }

        private void Open(){
            if(File.Exists(FilePath)){
                Text = fileService.Load(FilePath);
                Status = $"Loaded {Path.GetFileName(FilePath)}";
            }else{
                Status = "File not found";
            }
        }

        private void Save(){
            fileService.Save(FilePath, Text);
            Status = $"Saved {Path.GetFileName(FilePath)}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
