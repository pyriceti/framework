using UnityEngine;

namespace PyricetiFramework.Tests.Runtime
{
  public class EngineObjectTest : EngineObject
  {
    protected override void Build()
    {
      base.Build();
      Debug.Log($"{GetStamp(true)} build");
    }

    protected override void SetupEarly()
    {
      base.SetupEarly();
      Debug.Log($"{GetStamp(true)} setupEarly");
    }
    
    protected override void Setup()
    {
      base.Setup();
      Debug.Log($"{GetStamp(true)} setup");
    }
    
    protected override void SetupLate()
    {
      base.SetupLate();
      Debug.Log($"{GetStamp(true)} setupLate");
    }
    
    protected override void Destroy()
    {
      base.SetupLate();
      Debug.Log($"{GetStamp(true)} destroy");
    }
  }
}