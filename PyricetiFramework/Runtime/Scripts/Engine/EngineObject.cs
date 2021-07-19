using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PyricetiFramework
{
  [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public abstract partial class EngineObject : MonoBehaviour
  {
    protected readonly List<CancellationTokenSource> aliveCtsList = new List<CancellationTokenSource>();
    
    private CancellationTokenSource initObjCts = new CancellationTokenSource();

    private bool isEngineSubscriber = false;
    private string stamp = null;
    
    /// <summary>
    /// isReady is set to true when setupLate is called
    /// </summary>
    protected bool isReady = false;

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once ConvertToConstant.Global
    protected bool forceInitIfDisabledOnBuild = false;

    private void Awake() => build();

    private void Start() => waitThen(() => EngineManager.IsEngineReady, () => initObj().Forget());

    private void OnDisable()
    {
      if (!isReady && forceInitIfDisabledOnBuild)
        waitThen(() => EngineManager.IsEngineReady, () => waitOneFrameThen(() => initObj().Forget()));
    }

    private void OnDestroy()
    {
      initObjCts.Cancel();
      initObjCts.Dispose();
      initObjCts = null;

      // Cleanup remaining alive cts
      int aliveCtsNb = aliveCtsList.Count;
      for (var i = 0; i < aliveCtsNb; i++)
      {
        CancellationTokenSource cts = aliveCtsList[i];
        cts.Cancel();
        cts.Dispose();
      }

      aliveCtsList.Clear();
      // ---------------------------


      // If the app is not closing, make sure to unsubscribe object upon destruction
      // EngineManager.IsEngineReady may be false in UnitTests context
      if (EngineManager.IsEngineReady && !EngineManager.IsAppQuitting && isEngineSubscriber)
        EngineManager.unsubscribe(this);

      destroy();
    }

    private async UniTaskVoid initObj()
    {
      setupEarly();
      await UniTask.NextFrame(initObjCts.Token);
      setup();
      await UniTask.NextFrame(initObjCts.Token);
      setupLate();
    }

    protected virtual void build() { }

    protected virtual void setupEarly() { }

    protected virtual void setup() { }

    protected virtual void setupLate() => isReady = true;

    public virtual void updateEngine() { }

    protected virtual void destroy() { }

    /// <summary>
    /// Retrieve stamp from object with Type info and optionally frame info.
    /// TODO: Allow to precise namespace with EditorPrefs param.
    /// </summary>
    /// <param name="showFrame"></param>
    /// <returns></returns>
    public virtual string getStamp(bool showFrame = false)
    {
      if (stamp == null)
        stamp = $" <color=#14E3C6>{GetType().Name}</color> | <b>{name}</b> |";

      return showFrame ? $" <color=#A851D4>{Time.frameCount}</color> {stamp}" : stamp;
    }

    protected void registerAliveCts(CancellationTokenSource cts) => aliveCtsList.Add(cts);

    public void setIsEngineSubscriber(bool val = true) => isEngineSubscriber = val;
  }
}