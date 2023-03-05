using UnityEngine;

namespace PyricetiFramework.Tests.Runtime
{
  public class MenuPanelTest : MenuPanel
  {
    [Header("MenuPanelTest")]
    // ---
    [SerializeField] private bool fadeIn = false;
    [SerializeField] private bool fadeOut = false;

    protected override void Setup()
    {
      base.Setup();
      
      EngineManager.Subscribe(this);
    }

    public override void UpdateEngine()
    {
      base.UpdateEngine();

      if (fadeIn)
      {
        Show(true);
        fadeIn = false;
      }
      else if (fadeOut)
      {
        Hide(true);
        fadeOut = false;
      }
    }
  }
}