using System;
using System.Collections.Generic;

namespace PyricetiFramework
{
  public static class ControllersProvider
  {
    private static readonly Dictionary<Type, EngineController> Controllers = new Dictionary<Type, EngineController>();

    public static void registerController(EngineController controller)
    {
      Controllers.Add(controller.GetType(), controller);
    }

    public static T getController<T>() where T : EngineController => (T) Controllers[typeof(T)];
  }
}