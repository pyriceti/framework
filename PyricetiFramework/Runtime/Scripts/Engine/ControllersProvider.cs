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
    private static void initDomain()
    {
      _controllers = null;
    }

    private static Dictionary<Type, EngineController> _controllers;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void init()
    {
      _controllers = new Dictionary<Type, EngineController>();
    }
    
    public static void registerController(EngineController controller)
    {
      _controllers.Add(controller.GetType(), controller);
    }

    public static T getController<T>() where T : EngineController => (T) _controllers[typeof(T)];
  }
}