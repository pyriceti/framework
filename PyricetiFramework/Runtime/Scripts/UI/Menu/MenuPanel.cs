using System;
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

    private eUIVisibilityState visibilityState;

    public eUIVisibilityState VisibilityState
    {
      get => this.visibilityState;
      private set
      {
        this.visibilityState = value;
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (this.visibilityState)
        {
          case eUIVisibilityState.visible:
            this.PanelShownEvt?.Invoke();
            break;
          case eUIVisibilityState.hidden:
            this.PanelHiddenEvt?.Invoke();
            break;
        }
      }
    }

    protected virtual AnimationParams FadeAnimParams => DefaultAnimationParams;

    /// <summary>
    /// True when panel is at least partially visible (set early during fade operations).
    /// It allows "loose" checks. For "strict" visibility checks, use <see cref="VisibilityState"/>.
    /// </summary>
    public bool IsVisible { get; private set; }

    // ReSharper disable EventNeverSubscribedTo.Global
    public event Action BeforePanelShownEvt;
    public event Action PanelShownEvt;
    public event Action BeforePanelHiddenEvt;
    public event Action PanelHiddenEvt;
    // ReSharper restore EventNeverSubscribedTo.Global

    protected override void Build()
    {
      base.Build();

      this.RegisterAliveCts(this.fadeCts);

      this.canvasGroup = this.GetComponent<CanvasGroup>();
      Assert.IsNotNull(this.canvasGroup);

      if (this.disableOnAwake) this.DisableCanvasGroup(true);

      this.VisibilityState = this.canvasGroup.alpha > 0 ? eUIVisibilityState.visible : eUIVisibilityState.hidden;
      this.IsVisible = this.VisibilityState != eUIVisibilityState.hidden;
    }

    protected virtual void PreShow() { }

    protected virtual void PostShow() { }

    protected virtual void PreHide() { }

    protected virtual void PostHide() { }

    public void Show(bool fade = false) => this.Show(this.FadeAnimParams, fade);

    // TODO: add warning logs when isVisible is true, with custom preprocessor PYRICETI_FRAMEWORK_DEBUG directive
    public void Show(AnimationParams animationParams, bool fade = false)
    {
      if (this.IsVisible)
        return;

      this.PreShow();
      this.BeforePanelShownEvt?.Invoke();

      if (fade)
      {
        this.FadeIn(animationParams, this.PostShow).Forget();
        return;
      }

      this.IsVisible = true;
      this.EnableCanvasGroup(true);
      this.VisibilityState = eUIVisibilityState.visible;
      this.PostShow();
    }

    public void Hide(bool fade = false) => this.Hide(this.FadeAnimParams, fade);

    public void Hide(AnimationParams animationParams, bool fade = false)
    {
      if (!this.IsVisible)
        return;

      this.PreHide();
      this.BeforePanelHiddenEvt?.Invoke();

      if (fade)
      {
        this.FadeOut(animationParams, this.PostHide).Forget();
        return;
      }

      this.IsVisible = false;
      this.DisableCanvasGroup(true);
      this.VisibilityState = eUIVisibilityState.hidden;
      this.PostHide();
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private async UniTaskVoid FadeIn(Action onFadedIn = null)
    {
      await this.FadeIn(this.FadeAnimParams, onFadedIn);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private async UniTask FadeIn(AnimationParams animationParams, Action onFadedIn = null)
    {
      if (!this.allowFadeInterruption &&
          this.VisibilityState is eUIVisibilityState.fadingIn or eUIVisibilityState.fadingOut)
      {
        Debug.LogWarning($"{this.GetStamp()} allowFadeInterruption is <b>false</b>, aborting fadeIn");
        return;
      }

      this.IsVisible = true;
      this.VisibilityState = eUIVisibilityState.fadingIn;

      float originA = this.canvasGroup.alpha;

      await Animate(y => this.canvasGroup.alpha = (1f - y) * originA + y, animationParams,
        TaskUtil.RefreshToken(ref this.fadeCts, this.aliveCtsList));

      this.EnableCanvasGroup();
      this.VisibilityState = eUIVisibilityState.visible;

      onFadedIn?.Invoke();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private async UniTaskVoid FadeOut(Action onFadedOut = null)
    {
      await this.FadeOut(this.FadeAnimParams, onFadedOut);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private async UniTask FadeOut(AnimationParams animationParams, Action onFadedOut = null)
    {
      if (!this.allowFadeInterruption &&
        this.VisibilityState is eUIVisibilityState.fadingIn or eUIVisibilityState.fadingOut)
      {
        Debug.LogWarning($"{this.GetStamp()} allowFadeInterruption is <b>false</b>, aborting fadeOut");
        return;
      }

      this.IsVisible = false;
      this.VisibilityState = eUIVisibilityState.fadingOut;

      float originA = this.canvasGroup.alpha;

      await Animate(y => this.canvasGroup.alpha = (1f - y) * originA, animationParams,
        TaskUtil.RefreshToken(ref this.fadeCts, this.aliveCtsList));

      this.DisableCanvasGroup();
      this.VisibilityState = eUIVisibilityState.hidden;

      onFadedOut?.Invoke();
    }

    protected virtual void EnableCanvasGroup(bool setAlpha = false)
    {
      if (setAlpha)
      {
        this.canvasGroup.alpha = 1f;
        this.IsVisible = true;
        this.VisibilityState = eUIVisibilityState.visible;
      }

      this.canvasGroup.interactable = true;
      this.canvasGroup.blocksRaycasts = true;
    }

    protected virtual void DisableCanvasGroup(bool setAlpha = false)
    {
      if (setAlpha)
      {
        this.canvasGroup.alpha = 0f;
        this.IsVisible = false;
        this.VisibilityState = eUIVisibilityState.hidden;
      }

      this.canvasGroup.interactable = false;
      this.canvasGroup.blocksRaycasts = false;
    }

    [ContextMenu("Show")]
    private void cm_Show()
    {
      this.canvasGroup = this.GetComponent<CanvasGroup>();
      Assert.IsNotNull(this.canvasGroup);
      this.EnableCanvasGroup(true);
    }

    [ContextMenu("Hide")]
    private void cm_Hide()
    {
      this.canvasGroup = this.GetComponent<CanvasGroup>();
      Assert.IsNotNull(this.canvasGroup);
      this.DisableCanvasGroup(true);
    }
  }

  public class MenuPanel<TController> : MenuPanel where TController : EngineController
  {
    // ReSharper disable once MemberCanBePrivate.Global
    protected TController controller;

    protected override void SetupEarly()
    {
      base.SetupEarly();

      this.controller = ControllersProvider.GetController<TController>();
      Assert.IsNotNull(this.controller);
    }
  }
}