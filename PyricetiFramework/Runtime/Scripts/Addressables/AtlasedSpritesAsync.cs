using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PyricetiFramework
{
  /// <summary>
  /// This class does not extend from AssetsAsync because its purpose is to enable multiple atlased sprites loading
  /// <b>from the same atlas</b>. Otherwise, we would have a list of atlases to provide, which isn't what we need here.
  /// </summary>
  [Serializable]
  public sealed class AtlasedSpritesAsync
  {
    [SerializeField] private List<AssetReferenceAtlasedSprite> spritesRefs = default;

    private AsyncOperationHandle<IList<Sprite>> spritesRefsHandle;

    private bool isSpritesLoading;

    public bool IsSpritesLoaded { get; set; }

    public List<Sprite> Sprites => !spritesRefsHandle.IsValid() ? null : spritesRefsHandle.Result.ToList();

    public async UniTask LoadSpritesAsync(string label, Action onLoaded, CancellationToken token = default)
    {
      var loadedRef = new AddressablesManager.Ref<bool>(IsSpritesLoaded);
      var loadingRef = new AddressablesManager.Ref<bool>(isSpritesLoading);

      AsyncOperationHandle<IList<Sprite>> handle =
        await AddressablesManager.LoadAsyncAssets<Sprite>(spritesRefs, label, loadedRef, loadingRef, token);

      IsSpritesLoaded = loadedRef.Value;
      isSpritesLoading = loadingRef.Value;

      if (handle.IsValid())
        spritesRefsHandle = handle;

      onLoaded.Invoke();
    }

    public void UnloadAssets(string label)
    {
      AddressablesManager.UnloadAssets(label, IsSpritesLoaded, spritesRefsHandle);
      IsSpritesLoaded = false;
    }

    private string GetStamp() => "<b>AtlasedSpritesAsync::</b>";
  }
}