using UnityEngine;

namespace PyricetiFramework
{
  public abstract class EngineController : EngineObject
  {
    protected override void Build()
    {
      base.Build();

      ControllersProvider.RegisterController(this);
    }
    
    public static T GetController<T>() where T : EngineController => ControllersProvider.GetController<T>();
  }

  public abstract class EngineController<TSettings> : EngineController where TSettings : AbstractSettings
  {
    [SerializeField] protected TSettings settings = default;

    public TSettings Settings => settings;
  }
}