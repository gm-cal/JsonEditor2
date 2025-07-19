using System.Windows.Input;
using ViewModels;

public static class EditorSettings{
    public static string IndentString { get; private set; } = new string(' ', 4);
    public static KeyGesture IndentGesture { get; private set; } = new KeyGesture(Key.Tab);
    public static KeyGesture UnindentGesture { get; private set; } = new KeyGesture(Key.Tab, ModifierKeys.Shift);

    public static void Apply(SettingsViewModel vm){
        switch(vm.IndentStyle){
            case "2 Spaces":
                IndentString = new string(' ', 2);
                break;
            case "4 Spaces":
                IndentString = new string(' ', 4);
                break;
            case "Tab":
                IndentString = "\t";
                break;
        }
        KeyGestureConverter conv = new KeyGestureConverter();
        if(conv.ConvertFromString(vm.IndentShortcut) is KeyGesture kg1){
            IndentGesture = kg1;
        }
        if(conv.ConvertFromString(vm.UnindentShortcut) is KeyGesture kg2){
            UnindentGesture = kg2;
        }
    }
}
