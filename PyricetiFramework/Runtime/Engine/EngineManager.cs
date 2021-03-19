using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PyricetiFramework
{
  public class EngineManager : MonoBehaviour
  {
    public static bool isEngineReady = false;

    private void Start()
    {
      StartCoroutine(bootstrap(Startup.mainScenes));
    }

    private IEnumerator bootstrap(IEnumerable<SceneReference> scenes)
    {
      yield return StartCoroutine(ScenesManager.Instance.loadScenes(scenes));
      isEngineReady = true;
    }
  }
}