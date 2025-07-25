using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using Microsoft.Win32;
using Services;
using Utils;

namespace ViewModels{
    // テキストエディタのViewModel。ファイル操作やJSON変換・整形などの機能を提供します。
    public class TextEditorViewModel : INotifyPropertyChanged{
        private readonly IFileService           fileService;            // ファイル操作サービス
        private readonly IJsonService           jsonService;            // JSON操作サービス
        public ITextLineInputService            LineService;            // テキスト行入力サービス
        private readonly ITextService           textService;            // テキスト操作サービス
        private string text                     = string.Empty;         // エディタ内のテキスト内容
        private List<TextLine> selectedLines    = new List<TextLine>(); // 選択された行のリスト
        private string filePath                 = string.Empty;         // 編集対象ファイルのパス
        private string status                   = string.Empty;         // ステータスメッセージ
        private int indentWidth                 = 4;                    // JSON整形時のインデント幅
        private string title                    = string.Empty;         // タブタイトル
        private static int newFileCounter       = 1;                    // 新規ファイル用カウンタ

        // --- コンストラクタ。依存サービスを受け取り、コマンドを初期化します。
        // fileService  ファイル操作サービス
        // jsonService  JSON操作サービス
        public TextEditorViewModel(
            IFileService fileService,
            IJsonService jsonService,
            ITextLineInputService lineInputService,
            ITextService textService
        ){
            this.fileService        = fileService;
            this.jsonService        = jsonService;
            LineService             = lineInputService;
            this.textService        = textService;
            ConvertCommand      = new RelayCommand(_ => Convert());
            FormatCommand       = new RelayCommand(_ => Format());
            ToUpperCamelCommand = new RelayCommand(_ => ToUpperCamel());
            ToSnakeCaseCommand  = new RelayCommand(_ => ToSnakeCase());
            OpenCommand         = new RelayCommand(_ => Open());
            SaveCommand         = new RelayCommand(_ => Save());
            Title = $"新規テキスト_{newFileCounter++}";
        }

        // エディタ内のテキスト内容
        public string Text{
            get{ return text; }
            set{ text = value; OnPropertyChanged(); }
        }

        // ステータスメッセージ
        public string Status{
            get{ return status; }
            set{ status = value; OnPropertyChanged(); }
        }

        
        public ICommand ConvertCommand          { get; }  // タブ区切りテキストをJSONに変換するコマンド
        public ICommand FormatCommand           { get; }  // JSONを整形するコマンド
        public ICommand ToUpperCamelCommand     { get; }  // プロパティ名をUpperCamelに変換するコマンド
        public ICommand ToSnakeCaseCommand      { get; }  // プロパティ名をsnake_caseに変換するコマンド
        public ICommand OpenCommand             { get; }  // ファイルを開くコマンド
        public ICommand SaveCommand             { get; }  // ファイルを保存するコマンド

        // JSON整形時のインデント幅
        public int IndentWidth{
            get{ return indentWidth; }
            set{ indentWidth = value; OnPropertyChanged(); }
        }

        // タブタイトル
        public string Title{
            get{ return title; }
            private set{ title = value; OnPropertyChanged(); }
        }

        // 編集対象ファイルのパス
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

        // テキストをタブ区切りからJSONへ変換します。
        private void Convert(){
            if(jsonService.TryConvertTabToJson(Text, IndentWidth, out string json, out string error)){
                Text = json;
                Status = "変換しました";
            }else{
                Status = error;
            }
        }

        // テキスト(JSON)を整形します。
        private void Format(){
            if(jsonService.TryFormatJson(Text, IndentWidth, out string formatted, out string error)){
                Text = formatted;
                Status = "整形しました";
            }else{
                Status = error;
            }
        }

        private void ToUpperCamel(){
            if(textService.TryToUpperCamel(Text, IndentWidth, out string converted, out string error)){
                Text = converted;
                Status = "UpperCamelに変換しました";
            }else{
                Status = error;
            }
        }

        private void ToSnakeCase(){
            if(textService.TryToSnakeCase(Text, IndentWidth, out string converted, out string error)){
                Text = converted;
                Status = "snake_caseに変換しました";
            }else{
                Status = error;
            }
        }

        // ファイルを読み込みます。
        private void Open(){
            OpenFileDialog dlg = new OpenFileDialog();
            if(dlg.ShowDialog() == true){
                FilePath = dlg.FileName;
                if(File.Exists(FilePath)){
                    Text = fileService.Load(FilePath);
                    Status = $"{Path.GetFileName(FilePath)} を読み込みました";
                }
            }
        }

        // テキストをファイルに保存します。
        private void Save(){
            SaveFileDialog dlg = new SaveFileDialog();
            if(!string.IsNullOrEmpty(FilePath)){
                dlg.FileName = Path.GetFileName(FilePath);
                dlg.InitialDirectory = Path.GetDirectoryName(FilePath);
            }
            if(dlg.ShowDialog() == true){
                FilePath = dlg.FileName;
                fileService.Save(FilePath, Text);
                Status = $"{Path.GetFileName(FilePath)} を保存しました";
            }
        }


        // プロパティ変更通知を発行します。
        /// name    プロパティ名
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
