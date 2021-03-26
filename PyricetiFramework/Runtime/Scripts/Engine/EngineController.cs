namespace PyricetiFramework
{
  public abstract class EngineController : EngineObject
  {
    protected override void build()
    {
      base.build();

      ControllersProvider.registerController(this);
    }
  }
}