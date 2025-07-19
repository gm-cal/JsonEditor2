using System.Windows.Controls;
using ViewModels;

namespace Controls{
    public partial class TabControls : UserControl{
        private TabControl tabControl => Tabs;
        public TabControls(){
            InitializeComponent();
        }

        public TextEdit? SelectedTextEdit => tabControl.SelectedContent as TextEdit;

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            if(tabControl.SelectedItem is TabItem){
                if(DataContext is MainViewModel vm){
                    vm.NewTabCommand.Execute(null);
                    tabControl.SelectedIndex = vm.Editors.Count - 1;
                }
            }
        }
    }
}
