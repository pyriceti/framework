using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PyricetiFramework
{
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class EngineManager : Singleton<EngineManager>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitDomain()
    {
      IsEngineReady = false;
      IsAppQuitting = false;
    }
    
    [Min(1)] [SerializeField] private int startEngineObjsArrSize = 1;

    public static bool IsEngineReady { private set; get; } = false;

    public static bool IsAppQuitting { private set; get; } = false;

    /// <summary>
    /// Store all subscribed engine objects references
    /// </summary>
    private EngineObject[] allEngineObjects;

    private int engineObjsCount = 0;

    protected override void OnAwake()
    {
      base.OnAwake();

      allEngineObjects = new EngineObject[startEngineObjsArrSize];
    }

    private void Start() => Bootstrap(Startup.mainScenes).Forget();

    private static async UniTaskVoid Bootstrap(IEnumerable<SceneReference> scenes)
    {
      await ScenesManager.LoadScenesAsync(scenes);
      IsEngineReady = true;
    }

    private void Update()
    {
      // ReSharper disable once ForCanBeConvertedToForeach
      for (var i = 0; i < engineObjsCount; i++)
      {
        allEngineObjects[i].UpdateEngine();
      }
    }
    
    private void OnApplicationQuit() => IsAppQuitting = true;

    public static void Subscribe(EngineObject obj)
    {
      Instance._subscribe(obj);
      obj.SetIsEngineSubscriber();
    }

    private void _subscribe(EngineObject obj)
    {
      // Array is full, needs to be resized: size is doubled
      if (engineObjsCount >= allEngineObjects.Length)
        Array.Resize(ref allEngineObjects, allEngineObjects.Length * 2);

      allEngineObjects[engineObjsCount++] = obj;
    }
    
    public static void Unsubscribe(EngineObject obj) => Instance._unsubscribe(obj);

    private void _unsubscribe(EngineObject obj)
    {
      int idx = Array.IndexOf(allEngineObjects, obj);

      if (idx <= -1)
      {
        Debug.LogWarning($"Tried to unsubscribe {obj.name} but not found in allEngineObjects, aborting");
        return;
      }

      allEngineObjects = allEngineObjects.RemoveAt(idx);
      engineObjsCount--;
    }
  }
}