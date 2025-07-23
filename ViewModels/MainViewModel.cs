using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Services;
using Utils;

namespace ViewModels{
    public class MainViewModel : INotifyPropertyChanged{
        private TextEditorViewModel? selectedEditor;
        public ObservableCollection<TextEditorViewModel> Editors{ get; }
        public ICommand NewTabCommand{ get; }
        public ICommand CloseTabCommand{ get; }
        private readonly IFileService fileService;
        private readonly IJsonService jsonService;
        private readonly ITextLineInputService lineService;

        public MainViewModel(IFileService fileService, IJsonService jsonService, ITextLineInputService lineService){
            this.fileService = fileService;
            this.jsonService = jsonService;
            this.lineService = lineService;
            Editors = new ObservableCollection<TextEditorViewModel>();
            NewTabCommand = new RelayCommand(_ => AddTab());
            CloseTabCommand = new RelayCommand(e => RemoveTab(e as TextEditorViewModel), e => e is TextEditorViewModel);
            AddTab();
        }

        // 編集中のテキストエディタ
        public TextEditorViewModel? SelectedEditor{
            get{ return selectedEditor; }
            set{ selectedEditor = value; OnPropertyChanged(); }
        }

        // タブを追加します。
        private void AddTab(){
            TextEditorViewModel editor = new TextEditorViewModel(fileService, jsonService, lineService);
            Editors.Add(editor);
            SelectedEditor = editor;
        }

        // タブを閉じます。
        private void RemoveTab(TextEditorViewModel? editor){
            if(editor == null) return;
            int index = Editors.IndexOf(editor);
            if(index >= 0){
                Editors.RemoveAt(index);
                if(Editors.Count == 0){
                    AddTab();
                }else{
                    SelectedEditor = Editors[Math.Min(index, Editors.Count - 1)];
                }
            }
        }

        // プロパティ変更通知を発行します。
        // name    プロパティ名
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
