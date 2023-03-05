using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PyricetiFramework.Tests.Runtime
{
  public class AddrTest : EngineObject
  {
    [SerializeField] private AtlasedSpriteAsync atlasedSpriteAsync = default;
    [SerializeField] private AtlasedSpritesAsync atlasedSpritesAsync = default;
    [SerializeField] private AudioClipAsync audioClipAsync = default;
    [SerializeField] private AudioClipsAsync audioClipsAsync = default;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    protected override void Build()
    {
      base.Build();

      spriteRenderer = GetComponent<SpriteRenderer>();
      audioSource = GetComponent<AudioSource>();
    }

    protected override void Setup()
    {
      base.Setup();
      
      // testAtlasedAddrLoading().Forget();
      // testAtlasedMultipleAddrLoading().Forget();
      // testAudioAddrLoading().Forget();
      testAudioMultipleAddrLoading().Forget();
    }

    private async UniTaskVoid testAtlasedAddrLoading()
    {
      Debug.Log($"{GetStamp()} atlasedSpriteAsync.IsAssetLoaded: {atlasedSpriteAsync.IsAssetLoaded}");
      
      // Load atlas
      await atlasedSpriteAsync.LoadAssetAsync("Example atlased sprite", () =>
      {
        Debug.Log($"{GetStamp()} atlasedSpriteAsync loaded!");
        Debug.Log($"{GetStamp()} atlasedSpriteAsync.IsAssetLoaded: {atlasedSpriteAsync.IsAssetLoaded}");
        Debug.Log($"{GetStamp()} atlasedSpriteAsync.Asset: {atlasedSpriteAsync.Asset}");
      });
      
      spriteRenderer.sprite = atlasedSpriteAsync.Asset;

      await UniTask.Delay(TimeSpan.FromSeconds(5f));
      
      spriteRenderer.sprite = null;
      atlasedSpriteAsync.UnloadAsset("Example atlased sprite");
      
      Debug.Log($"{GetStamp()} atlasedSpriteAsync unloaded!");
      Debug.Log($"{GetStamp()} atlasedSpriteAsync.IsAssetLoaded: {atlasedSpriteAsync.IsAssetLoaded}");
    }
    
    private async UniTaskVoid testAtlasedMultipleAddrLoading()
    {
      Debug.Log($"{GetStamp()} atlasedSpritesAsync.IsSpritesLoaded: {atlasedSpritesAsync.IsSpritesLoaded}");
      
      // Load atlas
      await atlasedSpritesAsync.LoadSpritesAsync("Example atlased sprites", () =>
      {
        Debug.Log($"{GetStamp()} atlasedSpritesAsync loaded!");
        Debug.Log($"{GetStamp()} atlasedSpritesAsync.IsSpritesLoaded: {atlasedSpritesAsync.IsSpritesLoaded}");
        Debug.Log($"{GetStamp()} atlasedSpritesAsync.Sprites: {atlasedSpritesAsync.Sprites}");
      });
      
      spriteRenderer.sprite = atlasedSpritesAsync.Sprites[1];

      await UniTask.Delay(TimeSpan.FromSeconds(5f));
      
      spriteRenderer.sprite = null;
      atlasedSpriteAsync.UnloadAsset("Example atlased sprites");
      
      Debug.Log($"{GetStamp()} atlasedSpritesAsync unloaded!");
      Debug.Log($"{GetStamp()} atlasedSpritesAsync.IsSpritesLoaded: {atlasedSpritesAsync.IsSpritesLoaded}");
    }
    
    private async UniTaskVoid testAudioAddrLoading()
    {
      Debug.Log($"{GetStamp()} audioClipAsync.IsAssetLoaded: {audioClipAsync.IsAssetLoaded}");
      
      // Load atlas
      await audioClipAsync.LoadAssetAsync("Example audio clip", () =>
      {
        Debug.Log($"{GetStamp()} audioClipAsync loaded!");
        Debug.Log($"{GetStamp()} audioClipAsync.IsAssetLoaded: {audioClipAsync.IsAssetLoaded}");
        Debug.Log($"{GetStamp()} audioClipAsync.Asset: {audioClipAsync.Asset}");
      });
      
      audioSource.Play();

      await UniTask.Delay(TimeSpan.FromSeconds(5f));
      
      audioClipAsync.UnloadAsset("Example audio clip");
      
      Debug.Log($"{GetStamp()} audioClipAsync unloaded!");
      Debug.Log($"{GetStamp()} audioClipAsync.IsAssetLoaded: {audioClipAsync.IsAssetLoaded}");
    }
    
    private async UniTaskVoid testAudioMultipleAddrLoading()
    {
      Debug.Log($"{GetStamp()} audioClipsAsync.IsAssetsLoaded: {audioClipsAsync.IsAssetsLoaded}");
      
      // Load atlas
      await audioClipsAsync.LoadAssetsAsync("Example audio clips", () =>
      {
        Debug.Log($"{GetStamp()} audioClipsAsync loaded!");
        Debug.Log($"{GetStamp()} audioClipsAsync.IsAssetsLoaded: {audioClipsAsync.IsAssetsLoaded}");
        Debug.Log($"{GetStamp()} audioClipsAsync.Assets: {audioClipsAsync.Assets}");
      });

      audioSource.clip = audioClipsAsync.Assets[0];
      audioSource.Play();
      await UniTask.Delay(TimeSpan.FromSeconds(2f));

      audioSource.clip = audioClipsAsync.Assets[1];
      audioSource.Play();

      await UniTask.Delay(TimeSpan.FromSeconds(5f));

      audioSource.clip = null;
      audioClipsAsync.UnloadAssets("Example audio clips");
      
      Debug.Log($"{GetStamp()} audioClipsAsync unloaded!");
      Debug.Log($"{GetStamp()} audioClipsAsync.IsAssetsLoaded: {audioClipsAsync.IsAssetsLoaded}");
    }
    
    [UnityTest]
    public IEnumerator AnAudioClipIsSuccessfullyAddrLoadedThenUnloaded()
    {
      // TODO: document on Unit Testing
      var go = new GameObject();
      go.AddComponent<AudioListener>();
      go.AddComponent<AudioSource>();
      // var addrTest = go.AddComponent<AddrTest>();
      // addrTest.SetTestData(audioClipAsync);
      //

      // Debug.Log($"audioClipAsync: {audioClipAsync}");

      yield return null;
      //
      // Debug.Log($"audioClipAsync.IsAssetLoaded: {addrTest.audioClipAsync.IsAssetLoaded}");
      //
      // Assert.IsFalse(addrTest.audioClipAsync.IsAssetLoaded);
      // Assert.IsNull(addrTest.audioClipAsync.Asset);
      //
      // // Load atlas
      // yield return addrTest.audioClipAsync.loadAssetAsync("Example audio clip", () =>
      // {
      //   Debug.Log($"{GetStamp()} audioClipAsync loaded!");
      //   Debug.Log($"{GetStamp()} audioClipAsync.IsAssetLoaded: {addrTest.audioClipAsync.IsAssetLoaded}");
      //   Debug.Log($"{GetStamp()} audioClipAsync.Asset: {addrTest.audioClipAsync.Asset}");
      // });
      //
      // Assert.IsNotNull(addrTest.audioClipAsync.Asset);
      // Assert.IsTrue(addrTest.audioClipAsync.IsAssetLoaded);
      //
      // addrTest.audioSource.Play();
      //
      // yield return new WaitForSeconds(4f);
      //
      // addrTest.audioClipAsync.unloadAsset("Example audio clip");
      //
      // Debug.Log($"{GetStamp()} audioClipAsync unloaded!");
      // Debug.Log($"{GetStamp()} audioClipAsync.IsAssetLoaded: {addrTest.audioClipAsync.IsAssetLoaded}");
      //
      // Assert.IsNull(addrTest.audioClipAsync.Asset);
      // Assert.IsFalse(addrTest.audioClipAsync.IsAssetLoaded);
    }
  }
}