using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace PyricetiFramework
{
  public static class AddressablesManager
  {
    private static readonly List<ILoader> SubscribedLoaders = new List<ILoader>();

    private const float LoadingTimeout = 15f;
    private const float LoadingSpinnerTimeout = 5f;
    
    private static int _loadingInstances = 0;
  
    private static int LoadingInstances
    {
      get => _loadingInstances;
      set
      {
        int oldLoadingInstances = _loadingInstances;
        _loadingInstances = value;
        
        // No more addr loading instance, hide all visible loaders
        if (_loadingInstances == 0)
        {
          foreach (ILoader l in SubscribedLoaders)
            l.Hide();
        }
        // From 0 to 1 loading instance, show all visible loaders
        else if (oldLoadingInstances == 0 && _loadingInstances > 0)
        {
          foreach (ILoader l in SubscribedLoaders)
            l.Show();
        }
      }
    }

    public static void RegisterLoader(ILoader loader) => SubscribedLoaders.Add(loader);
    
    public static void RemoveLoader(ILoader loader) => SubscribedLoaders.Remove(loader);
    
    private enum eLoadMode
    {
      single,
      multiple,
      multiple_same_type
    }

    public static async UniTask<AsyncOperationHandle<T>> LoadAsyncAsset<T>(AssetReference assetRef, string label,
      Ref<bool> isLoadedFlag, Ref<bool> isLoadingFlag, Action<T> onAssetLoaded = null)
      where T : Object
    {
      void onLoaded(AsyncOperationHandle operationHandle)
      {
        var asset = (T) operationHandle.Result;
        onAssetLoaded?.Invoke(asset);
      }

      AsyncOperationHandle handle = await LoadAssets<T>(label, isLoadedFlag, isLoadingFlag, eLoadMode.single, assetRef,
        onLoaded, CancellationToken.None);

      return handle.IsValid() ? handle.Convert<T>() : default;
    }

    public static async UniTask<AsyncOperationHandle<IList<Object>>> LoadAsyncAssets(IEnumerable assetsRuntimeKeys,
      string label, Ref<bool> isLoadedFlag, Ref<bool> isLoadingFlag, CancellationToken token = default)
    {
      AsyncOperationHandle handle = await LoadAssets<Object>(label, isLoadedFlag, isLoadingFlag, eLoadMode.multiple,
        assetsRuntimeKeys, null, token);

      return handle.IsValid() ? handle.Convert<IList<Object>>() : default;
    }

    public static async UniTask<AsyncOperationHandle<IList<T>>> LoadAsyncAssets<T>(IEnumerable assetsRuntimeKeys,
      string label, Ref<bool> isLoadedFlag, Ref<bool> isLoadingFlag, CancellationToken token = default) where T : Object
    {
      AsyncOperationHandle handle = await LoadAssets<T>(label, isLoadedFlag, isLoadingFlag, eLoadMode.multiple_same_type,
        assetsRuntimeKeys, null, token);

      return handle.IsValid() ? handle.Convert<IList<T>>() : default;
    }

    public static void UnloadAsset(string label, bool isLoaded, AsyncOperationHandle handle)
    {
      if (!isLoaded)
      {
        Debug.LogWarning($"{GetStamp()}{label} not loaded, cannot unload");
        return;
      }

      Addressables.Release(handle);
      Debug.Log($"{GetStamp()}<b>UNLOADED</b> {label}");
    }

    public static void UnloadAssets(string label, bool isLoaded, AsyncOperationHandle handle) =>
      UnloadAsset(label, isLoaded, handle);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label"></param>
    /// <param name="isLoadedFlag"></param>
    /// <param name="isLoadingFlag"></param>
    /// <param name="mode"></param>
    /// <param name="assetNameOrRefOrKeys"></param>
    /// <param name="onLoaded"></param>
    /// <param name="token"></param>
    /// <typeparam name="T">Only used when mode = single</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static async UniTask<AsyncOperationHandle> LoadAssets<T>(string label, Ref<bool> isLoadedFlag,
      Ref<bool> isLoadingFlag, eLoadMode mode, object assetNameOrRefOrKeys,
      Action<AsyncOperationHandle> onLoaded = null, CancellationToken token = default)
      where T : Object
    {
      Debug.Log($"{GetStamp()}<b>LOADING {label}</b>");

      if (isLoadedFlag.Value || isLoadingFlag.Value)
      {
        Debug.Log($"{GetStamp()}<b>{label}</b> is already loaded/loading, aborting");
        return default;
      }

      isLoadingFlag.Value = true;

      AsyncOperationHandle handle = default;

      try
      {
        switch (mode)
        {
          case eLoadMode.single:
            switch (assetNameOrRefOrKeys)
            {
              case string assetName:
                handle = Addressables.LoadAssetAsync<T>(assetName);
                break;
              case AssetReference assetReference:
                handle = assetReference.LoadAssetAsync<T>();
                break;
              default:
                throw new ArgumentException("Bad type for passed assetNameOrRefOrKeys");
            }

            break;
          case eLoadMode.multiple:
            var assetsRuntimeKeys = (IEnumerable) assetNameOrRefOrKeys;
            handle = Addressables.LoadAssetsAsync<Object>(assetsRuntimeKeys, null, Addressables.MergeMode.Union);
            break;

          case eLoadMode.multiple_same_type:
            var assetsSameTypeRuntimeKeys = (IEnumerable) assetNameOrRefOrKeys;
            handle = Addressables.LoadAssetsAsync<T>(assetsSameTypeRuntimeKeys, null, Addressables.MergeMode.Union);
            break;

          default:
            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
      }
      catch (Exception e)
      {
        LOGLoadingException(handle, e);
        isLoadingFlag.Value = false;
        return default;
      }

      var t = 0f;
      var lastPercentComplete = 0f;
      var spinnerT = 0f;
      var isLoadingInstancesInc = false;
    
      while (!handle.IsDone)
      {
        spinnerT += Time.deltaTime;

        // Show spinner
        if (spinnerT > LoadingSpinnerTimeout && !isLoadingInstancesInc)
        {
          LoadingInstances++;
          isLoadingInstancesInc = true;
        }

        // If same percentComplete, increment t
        if (Math.Abs(lastPercentComplete - handle.PercentComplete) < float.Epsilon)
        {
          t += Time.deltaTime;
        }
        // Otherwise reset it
        else
        {
          t = 0;
        }

        // Timeout, log warning
        if (t > LoadingTimeout)
        {
          Debug.LogWarning(
            $"{GetStamp()}<b>{label}</b> took more than {LoadingTimeout:F}s to complete, " +
            $"staying at {lastPercentComplete:F} completion, aborting now");
          break;
        }    
        
        lastPercentComplete = handle.PercentComplete;
        await UniTask.NextFrame(cancellationToken: token);
      }

      LoadingInstances--;

      if (handle.Status != AsyncOperationStatus.Succeeded)
      {
        LOGLoadingException(handle);
        isLoadingFlag.Value = false;
        return default;
      }
    
      Debug.Log($"{GetStamp()}<b>LOADED {label}</b>");

      onLoaded?.Invoke(handle);

      isLoadingFlag.Value = false;
      isLoadedFlag.Value = true;

      return handle;
    }

    private static void LOGLoadingException(AsyncOperationHandle handle, Exception e = null)
    {
      if (e != null)
        Debug.LogError(e.ToString());
      Debug.LogError(
        $"{GetStamp()} <b>Failed to load some addr assets<b/>" + "\n" +
        "Status: " + handle.Status + "\n" +
        "OperationException: " +
        (handle.OperationException == null ? "[Empty]" : handle.OperationException.ToString()) + "\n" +
        "PercentComplete: " + handle.PercentComplete
      );
    }
    
    private static string GetStamp(bool showFrame = false) =>
      $"{(showFrame ? Time.frameCount.ToString() : "")} <color=#AA69CB>{nameof(AddressablesManager)}::</color>";

    /// <summary>
    /// Must be used to pass a inner type/class as ref param to loading/unloading methods of this class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Ref<T>
    {
      public T Value { get; set; }

      /// <summary>
      /// Basic constructor of the class.
      /// </summary>
      /// <param name="reference"></param>
      public Ref(T reference = default) => Value = reference;
    }
  }
}