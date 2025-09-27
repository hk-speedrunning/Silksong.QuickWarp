using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace QuickWarp;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.hk-speedrunning.quickwarp")]
public partial class QuickWarpPlugin : BaseUnityPlugin
{
    private ConfigEntry<string> quickWarpKeyName;
    public static KeyCode quickWarpKey = KeyCode.F5;
    
    public static QuickWarpPlugin Instance;
    
    private void Awake()
    {
        Instance = this;
        quickWarpKeyName = Config.Bind("General", "Toggle QuickWarp menu", "F5");
    }
    
    private void Start()
    {
        Warp.BuildRefs();
        
        var go = new GameObject("QuickWarp");
        DontDestroyOnLoad(go);
        go.AddComponent<QuickWarpGUI>();

        if (!KeyCode.TryParse(quickWarpKeyName.Value, true, out quickWarpKey))
        {
            Logger.LogError($"Failed to parse QuickWarp key value {quickWarpKeyName.Value}, defaulting to F5");
            quickWarpKey = KeyCode.F5;
        }
        
    }
}