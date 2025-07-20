using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Controls {
    /// <summary>
    /// Represents a single editable line in the editor.
    /// </summary>
    public class TextLine : INotifyPropertyChanged {
        private int lineNumber;
        private string text = string.Empty;

        /// <summary>行番号。</summary>
        public int LineNumber {
            get => lineNumber;
            set { lineNumber = value; OnPropertyChanged(); }
        }

        /// <summary>行のテキスト。</summary>
        public string Text {
            get => text;
            set { text = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
