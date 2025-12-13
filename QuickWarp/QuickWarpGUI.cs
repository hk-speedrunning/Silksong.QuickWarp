using UnityEngine;

namespace QuickWarp;

public class QuickWarpGUI : MonoBehaviour
{
    public bool Enabled;

    private Vector2 _mapScrollVector = Vector2.zero;
    private Vector2 _sceneScrollVector = Vector2.zero;
    private Vector2 _transitionScrollVector = Vector2.zero;

    private int _areaSelection;
    private int _sceneSelection;
    private int _transitionSelection;

    private string[] _areaNames = [];
    private string[] _sceneNames = [];
    private string[] _transitionNames = [];

    public void Awake()
    {
        _areaNames = Warp.GetAreaNames();
        ResetSceneNames();
        ResetTransitionNames();
    }

    public void Update()
    {
        if (Input.GetKeyDown(QuickWarpPlugin.quickWarpKey))
        {
            Enabled = !Enabled;
        }
    }

    public void OnGUI()
    { 
        if (GameManager.SilentInstance?.IsNonGameplayScene() == true) return;
        if (!Enabled) return;

        GUILayout.BeginArea(new Rect(550, 25, 520, 800));
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(140)); // Area
        _mapScrollVector = GUILayout.BeginScrollView(_mapScrollVector);
        var areaSelection = GUILayout.SelectionGrid(_areaSelection, _areaNames, 1);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.MaxHeight(400), GUILayout.MinWidth(150), GUILayout.MaxWidth(230), GUILayout.ExpandWidth(false)); // Scene
        _sceneScrollVector = GUILayout.BeginScrollView(_sceneScrollVector);
        var sceneSelection = GUILayout.SelectionGrid(_sceneSelection, _sceneNames, 1);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.MaxHeight(400)); // Transition
        _transitionScrollVector = GUILayout.BeginScrollView(_transitionScrollVector);
        _transitionSelection = GUILayout.SelectionGrid(_transitionSelection, _transitionNames, 1);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (_transitionSelection != 0)
        {
            Enabled = false;
            Warp.TryWarp(_sceneNames[sceneSelection], _transitionNames[_transitionSelection]);
            _transitionSelection = 0;
        }

        if (_sceneSelection != sceneSelection)
        {
            _sceneSelection = sceneSelection;
            ResetTransitionNames();
        }

        if (areaSelection != _areaSelection)
        {
            _areaSelection = areaSelection;
            ResetSceneNames();
            ResetTransitionNames();
        }
    }

    private void ResetSceneNames()
    {
        _sceneNames = Warp.GetSceneNames(_areaNames[_areaSelection]);
        _sceneSelection = 0;
    }

    private void ResetTransitionNames()
    {
        _transitionNames = ["Select", ..Warp.GetTransitionNames(_sceneNames[_sceneSelection])];
        _transitionSelection = 0;
    }
}
