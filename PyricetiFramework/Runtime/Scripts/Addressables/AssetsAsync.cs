using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace PyricetiFramework
{
  [Serializable]
  public abstract class AssetsAsync<TAsset, TAssetRef> where TAsset : Object where TAssetRef : AssetReference
  {
    [SerializeField] private List<TAssetRef> assetsRefs = default;

    private AsyncOperationHandle<IList<Object>> assetsRefsHandle;

    private bool isAssetsLoading;

    public bool IsAssetsLoaded { get; set; }

    public List<TAsset> Assets
    {
      get
      {
        if (!assetsRefsHandle.IsValid())
          return null;

        var assetsList = new List<TAsset>(assetsRefsHandle.Result.Count);

        foreach (Object o in assetsRefsHandle.Result)
        {
          var c = o as TAsset;
          if (c == null)
          {
            Debug.LogError($"{GetStamp()}Error: couldn't convert addressable res to {typeof(TAsset)}");
            continue;
          }

          assetsList.Add(c);
        }

        return assetsList;
      }
    }

    public async UniTask LoadAssetsAsync(string label, Action onLoaded, CancellationToken token = default)
    {
      var loadedRef = new AddressablesManager.Ref<bool>(IsAssetsLoaded);
      var loadingRef = new AddressablesManager.Ref<bool>(isAssetsLoading);

      AsyncOperationHandle<IList<Object>> handle =
        await AddressablesManager.LoadAsyncAssets(assetsRefs, label, loadedRef, loadingRef, token);

      IsAssetsLoaded = loadedRef.Value;
      isAssetsLoading = loadingRef.Value;

      if (handle.IsValid())
        assetsRefsHandle = handle;

      onLoaded.Invoke();
    }

    protected virtual void BeforeUnloadAsset() { }

    public void UnloadAssets(string label)
    {
      BeforeUnloadAsset();
      AddressablesManager.UnloadAssets(label, IsAssetsLoaded, assetsRefsHandle);
      IsAssetsLoaded = false;
    }

    protected virtual string GetStamp() => "<b>AssetsAsync::</b>";
  }
}