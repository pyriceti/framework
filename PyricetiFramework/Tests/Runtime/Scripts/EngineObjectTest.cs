using UnityEngine;

namespace PyricetiFramework.Tests.Runtime
{
  public class EngineObjectTest : EngineObject
  {
    protected override void build()
    {
      base.build();
      Debug.Log($"{getStamp(true)} build");
    }

    protected override void setupEarly()
    {
      base.setupEarly();
      Debug.Log($"{getStamp(true)} setupEarly");
    }
    
    protected override void setup()
    {
      base.setup();
      Debug.Log($"{getStamp(true)} setup");
    }
    
    protected override void setupLate()
    {
      base.setupLate();
      Debug.Log($"{getStamp(true)} setupLate");
    }
    
    protected override void destroy()
    {
      base.setupLate();
      Debug.Log($"{getStamp(true)} destroy");
    }
  }
}