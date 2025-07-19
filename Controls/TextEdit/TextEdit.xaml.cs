using System.Windows;
using System.Windows.Controls;
using ViewModels;

namespace Controls{
    public partial class TextEdit : UserControl{
        public TextEdit(){
            InitializeComponent();
        }

        private void OnDrop(object sender, DragEventArgs e){
            if(DataContext is TextEditorViewModel vm){
                if(e.Data.GetDataPresent(DataFormats.FileDrop)){
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if(files.Length > 0){
                        vm.FilePath = files[0];
                        vm.OpenCommand.Execute(null);
                    }
                }
            }
        }
    }
}
