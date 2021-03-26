using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PyricetiFramework
{
  public abstract partial class EngineObject
  {
    // ReSharper disable once MemberCanBePrivate.Global
    protected static readonly AnimationCurve EaseCurve01 = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // ReSharper disable once MemberCanBePrivate.Global
    protected static readonly AnimationCurve LinearCurve01 = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
    protected sealed class AnimationParams
    {
      public AnimationCurve curve = EaseCurve01;
      public float speed = 1f;
      public Func<bool> stopPredicate = null;
    }

    protected static async UniTask animate(Action<float> anim, AnimationParams animParams = null,
      CancellationToken token = default) => await animate((y, t) => anim(y), animParams, token);

    protected static async UniTask animate(Action<float, float> anim, AnimationParams animParams = null,
      CancellationToken token = default)
    {
      if (animParams == null)
        animParams = new AnimationParams();

      AnimationCurve curve = animParams.curve;
      var t = 0f;
      while (t <= 1)
      {
        // Evaluate stopPredicate if any
        if (animParams.stopPredicate?.Invoke() ?? false)
          return;

        float y = curve.Evaluate(t);
        anim.Invoke(y, t);

        t += Time.deltaTime * animParams.speed;
        await UniTask.NextFrame(token);
      }

      // Last step (when t == 1f)
      anim.Invoke(curve.Evaluate(1f), 1f);
    }
  }
}