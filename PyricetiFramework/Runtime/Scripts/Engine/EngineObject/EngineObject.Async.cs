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
    protected void waitXSecondsThen(float seconds, Action action, CancellationToken token = default) =>
      waitXSecondsThenAsync(seconds, action, token).Forget();

    protected static async UniTaskVoid waitXSecondsThenAsync(float seconds, Action action,
      CancellationToken token = default)
    {
      await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);
      action.Invoke();
    }

    protected static void waitEndOfFrameThen(Action action, CancellationToken token = default) =>
      waitEndOfFrameThenAsync(action, token).Forget();

    protected static async UniTaskVoid waitEndOfFrameThenAsync(Action action, CancellationToken token = default)
    {
      await UniTask.WaitForEndOfFrame(token);
      action.Invoke();
    }

    protected static void waitOneFrameThen(Action action, CancellationToken token = default) =>
      waitXFramesThenAsync(1, action, token).Forget();

    protected static void waitXFramesThen(int framesCount, Action action, CancellationToken token = default) =>
      waitXFramesThenAsync(framesCount, action, token).Forget();

    private static async UniTaskVoid waitXFramesThenAsync(int framesCount, Action action,
      CancellationToken token = default)
    {
      await UniTask.DelayFrame(framesCount, cancellationToken: token);
      action.Invoke();
    }

    protected static void waitThen(Func<bool> predicate, Action action, CancellationToken token = default) =>
      waitThenAsync(predicate, action, token).Forget();

    private static async UniTaskVoid waitThenAsync(Func<bool> predicate, Action action, CancellationToken token)
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