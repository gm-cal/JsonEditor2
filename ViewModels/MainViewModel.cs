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

        public MainViewModel(IFileService fileService, IJsonService jsonService){
            this.fileService = fileService;
            this.jsonService = jsonService;
            Editors = new ObservableCollection<TextEditorViewModel>();
            NewTabCommand = new RelayCommand(_ => AddTab());
            CloseTabCommand = new RelayCommand(e => RemoveTab(e as TextEditorViewModel), e => e is TextEditorViewModel);
            AddTab();
        }

        public TextEditorViewModel? SelectedEditor{
            get{ return selectedEditor; }
            set{ selectedEditor = value; OnPropertyChanged(); }
        }

        private void AddTab(){
            TextEditorViewModel editor = new TextEditorViewModel(fileService, jsonService);
            Editors.Add(editor);
            SelectedEditor = editor;
        }

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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
