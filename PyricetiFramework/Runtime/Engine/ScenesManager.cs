using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PyricetiFramework
{
  public class ScenesManager : Singleton<ScenesManager>
  {
    public const string BaseScenesPath = "Scenes/";

    public IEnumerator loadScenes(IEnumerable<SceneReference> scenes)
    {
      return scenes
        .ToList()
        .Select(scene => StartCoroutine(loadScene(scene)))
        .GetEnumerator();
    }

    public static IEnumerator loadScene(string scenePath)
    {
      if (isSceneLoaded(scenePath))
      {
        Debug.LogWarning($"Scene {scenePath} is already loaded");
        yield break;
      }

      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);

      yield return new WaitUntil(() => asyncLoad.isDone);
    }

    private IEnumerator loadScene(SceneReference scene)
    {
      yield return StartCoroutine(loadScene(scene.ScenePath));
    }

    public static void loadEngineScene() => SceneManager.LoadScene("Scenes/Engine", LoadSceneMode.Additive);

    public static void unloadScene(Scene scene) => SceneManager.UnloadSceneAsync(scene);

    private static bool isSceneLoaded(SceneReference scene) => isSceneLoaded(scene.ScenePath);

    private static bool isSceneLoaded(string scenePath)
    {
      int countLoaded = SceneManager.sceneCount;

      for (var i = 0; i < countLoaded; i++)
      {
        Scene loadedScene = SceneManager.GetSceneAt(i);
        if (scenePath == loadedScene.path) return true;
      }

      return false;
    }
  }
}