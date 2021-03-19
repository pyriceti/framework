using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PyricetiFramework.Editor
{
  /// <summary>
  /// Must be located in an Editor folder, since it works with UnityEditor.XXX namespaces.
  /// </summary>
  [InitializeOnLoad]
  internal class EditorStartup : ScriptableObject
  {
    #region Consts and readonly
    
    private const string editorPrefsEnableStartup = "com.pyriceti.framework.enableStartup";
    private const string DefaultStartupDataPath = "Assets/Data/StartupData.asset";

    #endregion

    [MenuItem("Edit/Force startup")]
    private static void forceStartup()
    {
      Debug.Log("Forcing startup again…");
      startup();
    }


    private static EditorStartup _instance = null;

    static EditorStartup()
    {
      EditorApplication.update += onInit;
    }

    private static void onInit()
    {
      if (!EditorPrefs.HasKey(editorPrefsEnableStartup) || !EditorPrefs.GetBool(editorPrefsEnableStartup))
        return;
      
      if (EditorApplication.isPlayingOrWillChangePlaymode)
        return;

      // ReSharper disable once DelegateSubtraction
      EditorApplication.update -= onInit;
      
      _instance = FindObjectOfType<EditorStartup>();
      if (_instance != null)
        return;
      
      _instance = CreateInstance<EditorStartup>();
      startup();
    }


    private static void update() { }

    private static void startup()
    {
      if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
        return;

      EditorApplication.update += update;

      Debug.Log("Launching Unity Editor for the \"superjeu\" project! :)");

      var startupData = AssetDatabase.LoadAssetAtPath<StartupData>(DefaultStartupDataPath);
      if (startupData == null)
      {
        Debug.LogWarning($"Couldn't load startup data at {DefaultStartupDataPath}, aborting.");
        return;
      }

      Debug.Log($"Loaded startup data at {DefaultStartupDataPath}");

      var loadedScenesPaths = new List<string>();

      int scenesCount = SceneManager.sceneCount;
      if (scenesCount > 1)
      {
        Debug.Log("Unloading all scenes from Hierarchy");
        for (int i = scenesCount - 1; i > 0; i--)
        {
          Scene loadedScene = SceneManager.GetSceneAt(i);
          loadedScenesPaths.Add(loadedScene.path);
          SceneManager.UnloadSceneAsync(loadedScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }
      }

      Debug.Log("Loading startup scene");
      EditorSceneManager.OpenScene(startupData.StartupScene.ScenePath);

      Debug.Log("Loading silently all other main scenes");
      startupData.MainScenes.ForEach(s =>
        EditorSceneManager.OpenScene(s.ScenePath, OpenSceneMode.AdditiveWithoutLoading));

      Debug.Log("Loading silently all previously opened scenes");
      loadedScenesPaths.ForEach(p => EditorSceneManager.OpenScene(p, OpenSceneMode.AdditiveWithoutLoading));

      Debug.Log("<b>Done!</b>");
    }
  }
}
