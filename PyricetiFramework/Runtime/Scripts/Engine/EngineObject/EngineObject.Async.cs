using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PyricetiFramework
{
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public abstract partial class EngineObject
  {
    protected void WaitXSecondsThen(float seconds, Action action, CancellationToken token = default) =>
      WaitXSecondsThenAsync(seconds, action, token).Forget();

    protected static async UniTaskVoid WaitXSecondsThenAsync(float seconds, Action action,
      CancellationToken token = default)
    {
      await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);
      action.Invoke();
    }

    protected static void WaitEndOfFrameThen(Action action, CancellationToken token = default) =>
      WaitEndOfFrameThenAsync(action, token).Forget();

    protected static async UniTaskVoid WaitEndOfFrameThenAsync(Action action, CancellationToken token = default)
    {
      await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
      action.Invoke();
    }

    protected static void waitOneFrameThen(Action action, CancellationToken token = default) =>
      WaitXFramesThenAsync(1, action, token).Forget();

    protected static void WaitXFramesThen(int framesCount, Action action, CancellationToken token = default) =>
      WaitXFramesThenAsync(framesCount, action, token).Forget();

    private static async UniTaskVoid WaitXFramesThenAsync(int framesCount, Action action,
      CancellationToken token = default)
    {
      await UniTask.DelayFrame(framesCount, cancellationToken: token);
      action.Invoke();
    }

    protected static void WaitThen(Func<bool> predicate, Action action, CancellationToken token = default) =>
      WaitThenAsync(predicate, action, token).Forget();

    private static async UniTaskVoid WaitThenAsync(Func<bool> predicate, Action action, CancellationToken token)
    {
      if (predicate.Invoke())
        action.Invoke();
      else
      {
        await UniTask.WaitUntil(predicate, cancellationToken: token);
        action.Invoke();  
      }
    }
  }
}