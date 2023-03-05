using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PyricetiFramework
{
  [Serializable]
  public class AtlasedSpriteAsync : AssetAsync<Sprite, AssetReferenceAtlasedSprite>
  {
    protected override string GetStamp() => "<b>AtlasedSpriteAsync::</b>";
  }
}