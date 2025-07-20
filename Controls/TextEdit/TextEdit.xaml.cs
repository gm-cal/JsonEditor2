using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Controls{
    public partial class TextEdit : UserControl{
        private ListBox lineList => Editor;
        public event EventHandler<int>? LineControlRequested;
        private bool internalChange = false;
        private readonly DispatcherTimer updateTimer = new();
        private readonly ObservableCollection<TextLine> lines = new();

        public ObservableCollection<TextLine> Lines => lines;

        public TextEdit(){
            InitializeComponent();
            updateTimer.Interval = TimeSpan.FromMilliseconds(300);
            updateTimer.Tick += UpdateTimer_Tick;
            DataContextChanged += OnDataContextChanged;
            EditorSettings.Changed += (_, _) => UpdateLineVisibility();
            UpdateLineVisibility();
        }
    }
}
