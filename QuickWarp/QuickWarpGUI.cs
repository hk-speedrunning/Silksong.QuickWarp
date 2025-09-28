using System.Linq;
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
        _sceneNames = Warp.GetSceneNames(_areaNames[0]).OrderBy(x => x).ToArray();
        _transitionNames = Warp.GetTransitionNames(_sceneNames[0]).OrderBy(x => x).ToArray();
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
        if (GameManager.instance?.IsNonGameplayScene() == true) return;
        if (!Enabled) return;

        GUILayout.BeginArea(new Rect(550, 25, 500, 800));
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(); // Area
        _mapScrollVector = GUILayout.BeginScrollView(_mapScrollVector);
        var areaSelection = GUILayout.SelectionGrid(_areaSelection, _areaNames, 1);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(); // Scene
        _sceneScrollVector = GUILayout.BeginScrollView(_sceneScrollVector);
        var sceneSelection = GUILayout.SelectionGrid(_sceneSelection, _sceneNames, 1);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(); // Transition
        _transitionScrollVector = GUILayout.BeginScrollView(_transitionScrollVector);
        _transitionSelection = GUILayout.SelectionGrid(_transitionSelection, _transitionNames, 1);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(); // Warp button
        var buttonPressed = GUILayout.Button("Warp");
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (buttonPressed)
        {
            Enabled = false;
            Warp.TryWarp(_sceneNames[sceneSelection], _transitionNames[_transitionSelection]);
        }

        if (_sceneSelection != sceneSelection)
        {
            _sceneSelection = sceneSelection;
            _transitionNames = Warp.GetTransitionNames(_sceneNames[_sceneSelection]).OrderBy(x => x).ToArray();
            _transitionSelection = 0;
        }

        if (areaSelection != _areaSelection)
        {
            _areaSelection = areaSelection;
            _sceneNames = Warp.GetSceneNames(_areaNames[_areaSelection]).OrderBy(x => x).ToArray();
            _sceneSelection = 0;
            _transitionNames = Warp.GetTransitionNames(_sceneNames[_sceneSelection]).OrderBy(x => x).ToArray();;
            _transitionSelection = 0;
        }
    }
}
