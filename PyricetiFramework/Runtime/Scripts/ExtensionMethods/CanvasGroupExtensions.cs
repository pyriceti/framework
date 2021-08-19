using UnityEngine;

namespace PyricetiFramework
{
  public static class CanvasGroupExtensions
  {
    public static void HideAndDisable(this CanvasGroup cg)
    {
      cg.alpha = 0f;
      cg.interactable = false;
      cg.blocksRaycasts = false;
    }

    public static void ShowAndEnable(this CanvasGroup cg)
    {
      cg.alpha = 1f;
      cg.interactable = true;
      cg.blocksRaycasts = true;
    }
  }
}