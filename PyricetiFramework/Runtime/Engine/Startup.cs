using System.Collections;
using UnityEngine;

namespace PyricetiFramework
{
  public class Startup : MonoBehaviour
  {
    public static SceneReference[] mainScenes;

    [SerializeField] private SceneReference[] scenes = default;

    private void Awake()
    {
      mainScenes = scenes;
    }

    private void Start()
    {
      StartCoroutine(waitForEngineThenUnload());
      ScenesManager.loadEngineScene();
    }


    private IEnumerator waitForEngineThenUnload()
    {
      yield return new WaitUntil(() => EngineManager.isEngineReady);
      ScenesManager.unloadScene(gameObject.scene);
    }
  }
}