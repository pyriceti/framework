using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PyricetiFramework
{
  public partial class EngineObject : MonoBehaviour
  {
    private void Awake() => build();

    private void Start() => waitThen(() => EngineManager.isEngineReady, () => initObj().Forget());

    private void Update() => updateEngine();

    private async UniTaskVoid initObj()
    {
      setupEarly();
      await UniTask.NextFrame();
      setup();
      await UniTask.NextFrame();
      setupLate();
    }

    protected virtual void build() { }

    protected virtual void setupEarly() { }

    protected virtual void setup() { }

    protected virtual void setupLate() { }

    protected virtual void updateEngine() { }
  }
}