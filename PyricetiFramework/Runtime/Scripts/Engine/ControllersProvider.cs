using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace PyricetiFramework
{
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public static class ControllersProvider
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitDomain()
    {
      _controllers = null;
    }

    private static Dictionary<Type, EngineController> _controllers;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
      _controllers = new Dictionary<Type, EngineController>();
    }
    
    public static void RegisterController(EngineController controller)
    {
      _controllers.Add(controller.GetType(), controller);
    }

    public static T GetController<T>() where T : EngineController
    {
      if (_controllers.TryGetValue(typeof(T), out EngineController res))
        return (T) res;

      return null;
    }
  }
}