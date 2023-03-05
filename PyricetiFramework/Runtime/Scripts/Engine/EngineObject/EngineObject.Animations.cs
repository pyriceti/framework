using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PyricetiFramework
{
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public abstract partial class EngineObject
  {
    // ReSharper disable once MemberCanBePrivate.Global
    protected static readonly AnimationCurve EaseCurve01 = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // ReSharper disable once MemberCanBePrivate.Global
    protected static readonly AnimationCurve LinearCurve01 = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    protected static readonly AnimationParams DefaultAnimationParams = new()
    {
      curve = EaseCurve01,
      speed = 1f,
      duration = null,
      stopPredicate = null,
      startingT = 0f,
    };

    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
    [Serializable]
    public struct AnimationParams
    {
      public AnimationCurve curve;
      public float speed;
      public float? duration;
      public Func<bool> stopPredicate;
      [Range(0f, 1f)] public float startingT;
    }

    protected static async UniTask Animate(Action<float> anim, AnimationParams animParams, CancellationToken token = default) =>
      await Animate((y, t) => anim(y), animParams, token);

    protected static async UniTask Animate(Action<float> anim, CancellationToken token = default) =>
      await Animate(anim, DefaultAnimationParams, token);

    protected static async UniTask Animate(Action<float, float> anim, AnimationParams animParams,
      CancellationToken token = default)
    {
      AnimationCurve curve = animParams.curve;
      float t = animParams.startingT;
      while (t <= 1)
      {
        // Evaluate stopPredicate if any
        if (animParams.stopPredicate?.Invoke() ?? false)
          return;

        float y = curve.Evaluate(t);
        anim.Invoke(y, t);

        if (animParams.duration.HasValue)
          t += Time.deltaTime * (1f / animParams.duration.Value);
        else
          t += Time.deltaTime * animParams.speed;
        await UniTask.NextFrame(token);
      }

      // Last step (when t == 1f)
      anim.Invoke(curve.Evaluate(1f), 1f);
    }
  }
}