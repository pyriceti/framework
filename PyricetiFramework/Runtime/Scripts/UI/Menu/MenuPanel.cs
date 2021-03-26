using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace PyricetiFramework
{
  // TODO: Handle custom animation params for fadeIn/Out with settings folder/scriptable object
  [RequireComponent(typeof(CanvasGroup))]
  [DisallowMultipleComponent]
  [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  public class MenuPanel : EngineObject
  {
    [Header("Menu Panel")]
    // ---
    [SerializeField] protected bool allowFadeInterruption = false;
    [SerializeField] protected bool disableOnAwake = true;

    private CanvasGroup canvasGroup;

    private CancellationTokenSource fadeCts = new CancellationTokenSource();

    /// <summary>
    /// True when panel is at least partially visible (set early during fade operations).
    /// It allows "loose" checks. For "strict" visibility checks, use <see cref="VisibilityState"/>.
    /// </summary>
    public bool IsVisible { get; private set; }

    public eUIVisibilityState VisibilityState { get; private set; }

    protected override void build()
    {
      base.build();

      registerAliveCts(fadeCts);

      canvasGroup = GetComponent<CanvasGroup>();
      Assert.IsNotNull(canvasGroup);

      if (disableOnAwake)
        disableCanvasGroup(true);
    }

    // TODO: add warning logs when isVisible is true, with custom preprocessor PYRICETI_FRAMEWORK_DEBUG directive
    public virtual void show(bool fade = false)
    {
      if (IsVisible)
        return;

      if (fade)
      {
        fadeIn().Forget();
        return;
      }

      IsVisible = true;
      enableCanvasGroup(true);
      VisibilityState = eUIVisibilityState.visible;
    }

    public virtual void hide(bool fade = false)
    {
      if (!IsVisible)
        return;

      if (fade)
      {
        fadeOut().Forget();
        return;
      }
      
      IsVisible = false;
      disableCanvasGroup(true);
      VisibilityState = eUIVisibilityState.hidden;
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private async UniTaskVoid fadeIn()
    {
      if (!allowFadeInterruption &&
        (VisibilityState == eUIVisibilityState.fadingIn || VisibilityState == eUIVisibilityState.fadingOut))
      {
        Debug.LogWarning($"{getStamp()} allowFadeInterruption is <b>false</b>, aborting fadeIn");
        return;
      }
      
      IsVisible = true;
      VisibilityState = eUIVisibilityState.fadingIn;

      float originA = canvasGroup.alpha;

      await animate(y => canvasGroup.alpha = (1f - y) * originA + y,
        token: TaskUtil.RefreshToken(ref fadeCts, aliveCtsList));

      enableCanvasGroup();
      VisibilityState = eUIVisibilityState.visible;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private async UniTaskVoid fadeOut()
    {
      if (!allowFadeInterruption &&
        (VisibilityState == eUIVisibilityState.fadingIn || VisibilityState == eUIVisibilityState.fadingOut))
      {
        Debug.LogWarning($"{getStamp()} allowFadeInterruption is <b>false</b>, aborting fadeOut");
        return;
      }
      
      IsVisible = false;
      VisibilityState = eUIVisibilityState.fadingOut;

      float originA = canvasGroup.alpha;

      await animate(y => canvasGroup.alpha = (1f - y) * originA,
        token: TaskUtil.RefreshToken(ref fadeCts, aliveCtsList));

      disableCanvasGroup();
      VisibilityState = eUIVisibilityState.hidden;
    }

    protected virtual void enableCanvasGroup(bool setAlpha = false)
    {
      if (setAlpha)
      {
        canvasGroup.alpha = 1f;
        IsVisible = true;
        VisibilityState = eUIVisibilityState.visible;
      }

      canvasGroup.interactable = true;
      canvasGroup.blocksRaycasts = true;
    }

    protected virtual void disableCanvasGroup(bool setAlpha = false)
    {
      if (setAlpha)
      {
        canvasGroup.alpha = 0f;
        IsVisible = false;
        VisibilityState = eUIVisibilityState.hidden;
      }

      canvasGroup.interactable = false;
      canvasGroup.blocksRaycasts = false;
    }
  }
}