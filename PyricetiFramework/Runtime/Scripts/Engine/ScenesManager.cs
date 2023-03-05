using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PyricetiFramework
{
  public class ScenesManager : Singleton<ScenesManager>
  {
    // TODO: move in custom window
    public const string BaseScenesPath = "Scenes/";

    public static async UniTask LoadScenesAsync(IEnumerable<SceneReference> scenes) =>
      await UniTask.WhenAll(scenes.Select(LoadSceneAsync));

    public static async UniTask LoadSceneAsync(string scenePath)
    {
      if (IsSceneLoaded(scenePath))
      {
        Debug.LogWarning($"Scene {scenePath} is already loaded");
        return;
      }

      await SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
    }

    private static async UniTask LoadSceneAsync(SceneReference scene) => await LoadSceneAsync(scene.ScenePath);

    // TODO: move editable path in custom window
    public static void LoadEngineScene()
    {
      // With "Assets/" prefix and ".unity" suffix
      var engineScenePath = "Assets/Scenes/Engine.unity";
      if (IsSceneLoaded(engineScenePath))
      {
        Debug.LogWarning($"Scene {engineScenePath} is already loaded");
        return;
      }

      // Without "Assets/" prefix
      SceneManager.LoadScene("Scenes/Engine", LoadSceneMode.Additive);
    }

    public static void UnloadScene(Scene scene) => SceneManager.UnloadSceneAsync(scene);

    private static bool IsSceneLoaded(SceneReference scene) => IsSceneLoaded(scene.ScenePath);

    private static bool IsSceneLoaded(string scenePath)
    {
      int countLoaded = SceneManager.sceneCount;

      for (var i = 0; i < countLoaded; i++)
      {
        Scene loadedScene = SceneManager.GetSceneAt(i);
        if (scenePath == loadedScene.path && loadedScene.isLoaded)
          return true;
      }

      return false;
    }
  }
}