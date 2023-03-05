using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#if UNITY_EDITOR
using System.Reflection;
#endif
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif

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

    private void Awake() => Build();

    private void Start() => WaitThen(() => EngineManager.IsEngineReady, () => InitObj().Forget());

    private void OnDisable()
    {
      if (!isReady && forceInitIfDisabledOnBuild)
        WaitThen(() => EngineManager.IsEngineReady, () => waitOneFrameThen(() => InitObj().Forget()));
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
        if (!cts.IsCancellationRequested) 
          cts.Cancel();
        cts.Dispose();
      }

      aliveCtsList.Clear();
      // ---------------------------


      // If the app is not closing, make sure to unsubscribe object upon destruction
      // EngineManager.IsEngineReady may be false in UnitTests context
      if (EngineManager.IsEngineReady && !EngineManager.IsAppQuitting && isEngineSubscriber)
        EngineManager.Unsubscribe(this);

      Destroy();
    }

    private async UniTaskVoid InitObj()
    {
      SetupEarly();
      await UniTask.NextFrame(initObjCts.Token);
      Setup();
      await UniTask.NextFrame(initObjCts.Token);
      SetupLate();
    }

    protected virtual void Build()
    {
#if UNITY_EDITOR
      var projectSettings = EngineConfigScriptableObject.GetActiveInstance<ProjectSettings>();
      if (projectSettings != null && !projectSettings.IsSerializedFieldCheckEnabled)
        return;

      // Check for unset component serialized references
      foreach (FieldInfo fieldInfo in GetType().GetFields(
        BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
      {
        if (!fieldInfo.IsPublic && fieldInfo.GetCustomAttribute<SerializeField>() == null ||
            fieldInfo.GetCustomAttribute<MayBeNullOnAwake>() != null ||
            !fieldInfo.FieldType.IsSubclassOf(typeof(Component)))
          continue;

        if (fieldInfo.GetValue(this) == null)
          Debug.LogError($"Serialized field <b>{fieldInfo.Name}</b> is null", this);
      }
#endif
    }

    protected virtual void SetupEarly() { }

    protected virtual void Setup() { }

    protected virtual void SetupLate() => isReady = true;

    public virtual void UpdateEngine() { }

    protected virtual void Destroy() { }

    /// <summary>
    /// Retrieve stamp from object with Type info and optionally frame info.
    /// TODO: Allow to precise namespace with EditorPrefs param.
    /// </summary>
    /// <param name="showFrame"></param>
    /// <returns></returns>
    public virtual string GetStamp(bool showFrame = false)
    {
      if (stamp == null)
        stamp = $" <color=#14E3C6>{GetType().Name}</color> | <b>{name}</b> |";

      return showFrame ? $" <color=#A851D4>{Time.frameCount}</color> {stamp}" : stamp;
    }

    protected void RegisterAliveCts(CancellationTokenSource cts) => aliveCtsList.Add(cts);

    public void SetIsEngineSubscriber(bool val = true) => isEngineSubscriber = val;
  }

  public abstract class EngineObject<TController> : EngineObject where TController : EngineController
  {
    // ReSharper disable once MemberCanBePrivate.Global
    protected TController controller;

    protected override void SetupEarly()
    {
      base.SetupEarly();

      controller = ControllersProvider.GetController<TController>();
      Assert.IsNotNull(controller);
    }
  }
}