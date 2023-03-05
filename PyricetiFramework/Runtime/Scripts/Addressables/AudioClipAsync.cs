using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PyricetiFramework
{
  /// <summary>
  /// Automatically handles assigning clip to source upon loaded,
  /// resetting it to null and stopping it when unloaded.
  /// </summary>
  [Serializable]
  public class AudioClipAsync : AssetAsync<AudioClip, AssetReference>
  {
    [SerializeField] private AudioSource src = default;

    private bool hasSrc;

    protected override void OnAssetLoaded()
    {
      hasSrc = src != null;

      if (hasSrc)
        src.clip = Asset;
    }

    protected override void BeforeUnloadAsset()
    {
      if (!hasSrc)
        return;

      src.Stop();
      src.clip = null;
    }

    public void Play()
    {
      if (hasSrc)
        src.Play();
      else Debug.LogError($"{GetStamp()} Missing src (AudioClipAsync.play)?");
    }
  }
}