using UnityEngine;

namespace PyricetiFramework.Tests.Runtime
{
  public class MenuPanelTest : MenuPanel
  {
    [Header("MenuPanelTest")]
    // ---
    [SerializeField] private bool fadeIn = false;
    [SerializeField] private bool fadeOut = false;

    protected override void setup()
    {
      base.setup();
      
      EngineManager.subscribe(this);
    }

    public override void updateEngine()
    {
      base.updateEngine();

      if (fadeIn)
      {
        show(true);
        fadeIn = false;
      }
      else if (fadeOut)
      {
        hide(true);
        fadeOut = false;
      }
    }
  }
}