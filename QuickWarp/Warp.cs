using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using JetBrains.Annotations;
using UnityEngine;

namespace QuickWarp;

public static class Warp
{
    public static Vector2 pos;
    
    private static Dictionary<string, SceneTeleportMap.SceneInfo> _scenes;
    private static Dictionary<MapZone, List<string>> _scenes_by_map_zone = new();
    private static Dictionary<string, List<string>> _transitions_by_scene = new(); 
    private static Dictionary<string, List<string>> _respawns_by_scene = new();
    
    
    private static MapZone _mapZone;
    private static string _sceneName;
    private static string _transitionName;

    public static List<MapZone> GetMapZones()
    {
        return _scenes_by_map_zone.Keys.ToList();
    }

    public static List<string> GetSceneNames(MapZone mapZone)
    {
        return _scenes_by_map_zone[mapZone];
    }

    public static List<string> GetTransitionNames(string sceneName)
    {
        return _transitions_by_scene[sceneName];
    }

    public static void BuildRefs()
    {
        _scenes = SceneTeleportMap.Instance.sceneList.RuntimeData;
        _scenes_by_map_zone.Clear();
        _transitions_by_scene.Clear();
        _respawns_by_scene.Clear();
        
        foreach (var (sceneName, sceneData) in _scenes)
        {
            _scenes_by_map_zone.TryAdd(sceneData.MapZone, []);
            _scenes_by_map_zone[sceneData.MapZone].Add(sceneName);

            _transitions_by_scene[sceneName] = sceneData.TransitionGates;
            _respawns_by_scene[sceneName] = sceneData.RespawnPoints;
        }
    }

    public static void TryWarp(string sceneName, string transitionName)
    {
        QuickWarpPlugin.Instance.StartCoroutine(TryWarpCoroutine(sceneName, transitionName));
    }

    public static IEnumerator TryWarpCoroutine(string sceneName, string transitionName)
    {
        if (GameManager.instance.IsGamePaused())
        {
            IEnumerator pauseGameIterator = GameManager.instance.PauseGameToggleByMenu();
            while (pauseGameIterator.MoveNext())
            {
                yield return pauseGameIterator.Current;
            }
        }
        
        GameManager.SceneLoadInfo sceneLoadInfo = new()
        {
            SceneName = sceneName,
            EntryGateName = transitionName,
            PreventCameraFadeOut = true,
            WaitForSceneTransitionCameraFade = false,
            Visualization = GameManager.SceneLoadVisualizations.Default,
            AlwaysUnloadUnusedAssets = true,
            IsFirstLevelForPlayer = false
        };
        GameManager.UnsafeInstance.BeginSceneTransition(sceneLoadInfo);
    }
}