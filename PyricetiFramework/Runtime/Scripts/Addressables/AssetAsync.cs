using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace PyricetiFramework
{
  [Serializable]
  public abstract class AssetAsync<TAsset, TAssetRef> where TAsset : Object where TAssetRef : AssetReference
  {
    [SerializeField] private TAssetRef assetRef = default;

    private AsyncOperationHandle<TAsset> assetRefHandle;

    private bool isAssetLoading;

    public bool IsAssetLoaded { get; set; }

    public TAsset Asset => assetRefHandle.IsValid() ? assetRefHandle.Result : null;

    public async UniTask LoadAssetAsync(string label, Action onLoaded = null)
    {
      var loadedRef = new AddressablesManager.Ref<bool>(IsAssetLoaded);
      var loadingRef = new AddressablesManager.Ref<bool>(isAssetLoading);

      AsyncOperationHandle<TAsset> handle =
        await AddressablesManager.LoadAsyncAsset<TAsset>(assetRef, label, loadedRef, loadingRef);

      IsAssetLoaded = loadedRef.Value;
      isAssetLoading = loadingRef.Value;

      if (handle.IsValid())
        assetRefHandle = handle;

      OnAssetLoaded();
      onLoaded?.Invoke();
    }
    
    public async UniTask LoadAssetAsync(string label, Action<TAsset> onLoaded = null)
    {
      var loadedRef = new AddressablesManager.Ref<bool>(IsAssetLoaded);
      var loadingRef = new AddressablesManager.Ref<bool>(isAssetLoading);

      AsyncOperationHandle<TAsset> handle =
        await AddressablesManager.LoadAsyncAsset<TAsset>(assetRef, label, loadedRef, loadingRef);

      IsAssetLoaded = loadedRef.Value;
      isAssetLoading = loadingRef.Value;

      if (handle.IsValid())
        assetRefHandle = handle;

      OnAssetLoaded();
      onLoaded?.Invoke(assetRefHandle.Result);
    }

    protected virtual void OnAssetLoaded() { }

    protected virtual void BeforeUnloadAsset() { }

    public void UnloadAsset(string label)
    {
      BeforeUnloadAsset();
      AddressablesManager.UnloadAssets(label, IsAssetLoaded, assetRefHandle);
      IsAssetLoaded = false;
    }

    protected virtual string GetStamp() => "<b>AssetAsync::</b>";
  }
}