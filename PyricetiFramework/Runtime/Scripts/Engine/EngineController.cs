using UnityEngine;

namespace PyricetiFramework
{
  public abstract class EngineController : EngineObject
  {
    protected override void build()
    {
      base.build();

      ControllersProvider.registerController(this);
    }
    
    public static T getController<T>() where T : EngineController => ControllersProvider.getController<T>();
  }

  public abstract class EngineController<TSettings> : EngineController where TSettings : AbstractSettings
  {
    [SerializeField] protected TSettings settings = default;

    public TSettings Settings => settings;
  }
}