using System.Collections.ObjectModel;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PyricetiFramework
{
  public class Startup : MonoBehaviour
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void initDomain()
    {
      mainScenes = null;
    }
    
    private CancellationTokenSource loadEngineSceneCts = new CancellationTokenSource();

    public static ReadOnlyCollection<SceneReference> mainScenes = null;

    [SerializeField] private SceneReference[] scenes = default;

    private void Awake() => mainScenes = new ReadOnlyCollection<SceneReference>(scenes);

    private void Start() => loadEngineSceneThenUnloadStartup().Forget();

    private async UniTaskVoid loadEngineSceneThenUnloadStartup()
    {
      ScenesManager.loadEngineScene(); // Load engine scene here (engine bootstrap entry point)
      await UniTask.WaitUntil(() => EngineManager.IsEngineReady, cancellationToken: loadEngineSceneCts.Token);
      ScenesManager.unloadScene(gameObject.scene);
    }

    private void OnDestroy()
    {
      loadEngineSceneCts.Cancel();
      loadEngineSceneCts.Dispose();
      loadEngineSceneCts = null;

    }
  }
}