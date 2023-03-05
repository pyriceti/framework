using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PyricetiFramework
{
  /// <summary>
  /// Automatically reset animator runtime src to null and disable it when unloaded.
  /// </summary>
  [Serializable]
  public class RuntimeAnimatorControllerAsync : AssetAsync<RuntimeAnimatorController, AssetReference>
  {
    [SerializeField] private Animator animator = default;

    protected override void BeforeUnloadAsset()
    {
      animator.enabled = false;
      animator.runtimeAnimatorController = null;
    }
  }
}