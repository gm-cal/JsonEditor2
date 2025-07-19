using System.IO;
using System.Text.Json;
using System.Windows.Input;
using ViewModels;

public static class EditorSettings{
    private const string ShortcutFile = "shortcuts.json";
    public static string IndentString { get; private set; } = new string(' ', 4);
    public static KeyGesture IndentGesture { get; private set; } = new KeyGesture(Key.Tab);
    public static KeyGesture UnindentGesture { get; private set; } = new KeyGesture(Key.Tab, ModifierKeys.Shift);
    public static bool ShowLineNumbers { get; private set; } = true;
    public static event EventHandler? Changed;

    static EditorSettings(){
        LoadShortcuts();
    }

    public static void Apply(SettingsViewModel vm){
        switch(vm.IndentStyle){
            case "2 Spaces":
                IndentString = new string(' ', 2);
                break;
            case "4 Spaces":
                IndentString = new string(' ', 4);
                break;
            case "Tab":
                IndentString = new string(' ', 4);
                break;
        }
        KeyGestureConverter conv = new KeyGestureConverter();
        if(conv.ConvertFromString(vm.IndentShortcut) is KeyGesture kg1){
            IndentGesture = kg1;
        }
        if(conv.ConvertFromString(vm.UnindentShortcut) is KeyGesture kg2){
            UnindentGesture = kg2;
        }
        ShowLineNumbers = vm.ShowLineNumbers;
        SaveShortcuts();
        Changed?.Invoke(null, EventArgs.Empty);
    }

    private static void LoadShortcuts(){
        if(!File.Exists(ShortcutFile)) return;
        try{
            ShortcutConfig? conf = JsonSerializer.Deserialize<ShortcutConfig>(File.ReadAllText(ShortcutFile));
            if(conf != null){
                KeyGestureConverter conv = new KeyGestureConverter();
                if(conv.ConvertFromString(conf.IndentShortcut) is KeyGesture kg1) IndentGesture = kg1;
                if(conv.ConvertFromString(conf.UnindentShortcut) is KeyGesture kg2) UnindentGesture = kg2;
            }
        }catch{}
    }

    private static void SaveShortcuts(){
        ShortcutConfig conf = new(){
            IndentShortcut = new KeyGestureConverter().ConvertToString(IndentGesture)!,
            UnindentShortcut = new KeyGestureConverter().ConvertToString(UnindentGesture)!
        };
        File.WriteAllText(ShortcutFile, JsonSerializer.Serialize(conf));
    }

    private class ShortcutConfig{
        public string IndentShortcut { get; set; } = "Tab";
        public string UnindentShortcut { get; set; } = "Shift+Tab";
    }
}
