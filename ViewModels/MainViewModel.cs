using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JsonEditor2.Services;
using Utils;

namespace JsonEditor2.ViewModels{
    public class MainViewModel : INotifyPropertyChanged{
        private TextEditorViewModel? selectedEditor;
        public ObservableCollection<TextEditorViewModel> Editors{ get; }
        public ICommand NewTabCommand{ get; }
        private readonly IFileService fileService;
        private readonly IJsonService jsonService;

        public MainViewModel(IFileService fileService, IJsonService jsonService){
            this.fileService = fileService;
            this.jsonService = jsonService;
            Editors = new ObservableCollection<TextEditorViewModel>();
            NewTabCommand = new RelayCommand(_ => AddTab());
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
