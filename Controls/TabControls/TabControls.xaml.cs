using System.Windows.Controls;
using JsonEditor2.ViewModels;

namespace JsonEditor2.Controls{
    public partial class TabControls : UserControl{
        public TabControls(){
            InitializeComponent();
        }

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
