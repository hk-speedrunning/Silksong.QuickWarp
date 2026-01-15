using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;

namespace QuickWarp;

public static class Warp
{
    
    private static Dictionary<string, SceneTeleportMap.SceneInfo> _scenes;
    private static Dictionary<string, List<string>> _scenes_by_area = new();
    private static Dictionary<string, List<string>> _transitions_by_scene = new(); 
    private static Dictionary<string, List<string>> _respawns_by_scene = new();


    private static MapZone _mapZone;
    private static string _sceneName;
    private static string _transitionName;

    public static string[] GetAreaNames()
    {
        return [.. _scenes_by_area.Keys];
    }

    public static string[] GetSceneNames(string areaName)
    {
        return [.. _scenes_by_area[areaName].OrderBy(x => x)];
    }

    public static string[] GetTransitionNames(string sceneName)
    {
        return [.. _transitions_by_scene[sceneName].OrderBy(x => x)];
    }

    public static void BuildRefs()
    {
        _scenes = SceneTeleportMap.Instance.sceneList.RuntimeData;
        _scenes_by_area = JsonUtil.Deserialize<Dictionary<string, List<string>>>("QuickWarp.Resources.scenes_by_area.json");
        _transitions_by_scene.Clear();
        _respawns_by_scene.Clear();
        
        foreach (var (sceneName, sceneData) in _scenes)
        {
            // if (!_scenes_by_area.SelectMany(kvp => kvp.Value).Contains(sceneName))
            // {
            //     QuickWarpPlugin.Log.LogInfo($"Unsorted scene: {sceneName}, {sceneData.MapZone}, {sceneData.TransitionGates.Count}, {sceneData.RespawnPoints.Count}");
            // }

            _transitions_by_scene[sceneName] = sceneData.TransitionGates;
            _respawns_by_scene[sceneName] = sceneData.RespawnPoints;
        }

        // foreach (var scene in _scenes_by_area.SelectMany(kvp => kvp.Value))
        // {
        //     if (!_scenes.ContainsKey(scene))
        //     {
        //         QuickWarpPlugin.Log.LogInfo($"Unused scene: {scene}");
        //     }
        // }
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