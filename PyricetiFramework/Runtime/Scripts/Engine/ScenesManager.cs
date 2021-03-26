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

    public static async UniTask loadScenesAsync(IEnumerable<SceneReference> scenes) =>
      await UniTask.WhenAll(scenes.Select(loadSceneAsync));

    public static async UniTask loadSceneAsync(string scenePath)
    {
      if (isSceneLoaded(scenePath))
      {
        Debug.LogWarning($"Scene {scenePath} is already loaded");
        return;
      }

      await SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
    }

    private static async UniTask loadSceneAsync(SceneReference scene) => await loadSceneAsync(scene.ScenePath);

    // TODO: move editable path in custom window
    public static void loadEngineScene()
    {
      // With "Assets/" prefix and ".unity" suffix
      var engineScenePath = "Assets/Scenes/Engine.unity";
      if (isSceneLoaded(engineScenePath))
      {
        Debug.LogWarning($"Scene {engineScenePath} is already loaded");
        return;
      }

      // Without "Assets/" prefix
      SceneManager.LoadScene("Scenes/Engine", LoadSceneMode.Additive);
    }

    public static void unloadScene(Scene scene) => SceneManager.UnloadSceneAsync(scene);

    private static bool isSceneLoaded(SceneReference scene) => isSceneLoaded(scene.ScenePath);

    private static bool isSceneLoaded(string scenePath)
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