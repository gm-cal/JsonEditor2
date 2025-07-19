using System.Windows.Controls;
using ViewModels;

namespace Controls{
    public partial class TabControls : UserControl{
        public TabControls(){
            InitializeComponent();
        }

        public TextEdit? SelectedTextEdit => Tabs.SelectedContent as TextEdit;

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            if(Tabs.SelectedItem is TabItem){
                if(DataContext is MainViewModel vm){
                    vm.NewTabCommand.Execute(null);
                    Tabs.SelectedIndex = vm.Editors.Count - 1;
                }
            }
        }
    }
}
