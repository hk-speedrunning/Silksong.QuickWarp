using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using UnityEngine;

namespace QuickWarp;

public class QuickWarpGUI : MonoBehaviour
{
    public bool Enabled;
    
    private Vector2 mapScrollVector = Vector2.zero;
    private Vector2 sceneScrollVector = Vector2.zero;
    private Vector2 transitionScrollVector = Vector2.zero;
    
    private int mapZoneSelection = 0;
    private int sceneSelection = 0;
    
    private List<MapZone> mapZones;
    private List<string> sceneNames = [];
    private string[] mapZoneSelectionStrings = [];
    private string[] sceneSelectionStrings = [];
    private string[] transitionSelectionStrings = [];
    
    public void Awake()
    {
        mapZones = Warp.GetMapZones();
        mapZoneSelectionStrings = mapZones.Select((x) => x.ToString()).ToArray();
    }

    public void Update()
    {
        if (Input.GetKeyDown(QuickWarpPlugin.quickWarpKey))
        {
            Enabled = !Enabled;
        }
    }
    
    public void OnGUI ()
    {
        if (GameManager.instance?.IsNonGameplayScene() == true) return;
        if (!Enabled) return;
        
        GUILayout.BeginArea(new Rect(550, 25, 500, 800));
        
            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(); // MapZone
                    mapScrollVector = GUILayout.BeginScrollView(mapScrollVector);
                        var _mapZoneSelection = GUILayout.SelectionGrid(mapZoneSelection, mapZoneSelectionStrings, 1);
                    GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginVertical(); // Scene
                    sceneScrollVector = GUILayout.BeginScrollView(sceneScrollVector);
                        var _sceneSelection = GUILayout.SelectionGrid(sceneSelection, sceneSelectionStrings, 1);
                    GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginVertical(); // Transition
                    transitionScrollVector = GUILayout.BeginScrollView(transitionScrollVector);
                        var _transitionSelection = GUILayout.SelectionGrid(0, transitionSelectionStrings, 1);
                    GUILayout.EndScrollView();
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        
        GUILayout.EndArea();

        if (_transitionSelection != 0)
        {
            Enabled = false;
            Warp.TryWarp(sceneSelectionStrings[sceneSelection], transitionSelectionStrings[_transitionSelection]);
        }

        if (_sceneSelection != sceneSelection)
        {
            UpdateScene(_sceneSelection);
            sceneSelection = _sceneSelection;
        }
        
        if (_mapZoneSelection != mapZoneSelection )
        {
            UpdateMapZone(_mapZoneSelection);
            mapZoneSelection = _mapZoneSelection;
            sceneSelection = 0;
        }
    }

    private void UpdateMapZone(int selection)
    {
        var zone = mapZones[selection];
        sceneNames = Warp.GetSceneNames(zone).OrderBy(x => x).ToList();
        sceneSelectionStrings = sceneNames.ToArray();
    }

    private void UpdateScene(int selection)
    {
        var scene = sceneNames[selection];

        transitionSelectionStrings = new List<string> { "Select" }.Concat(Warp.GetTransitionNames(scene).OrderBy(x => x)).ToArray();
    }
    
    
}