using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace PyricetiFramework
{
  public static class AssetUtil
  {
    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
      string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
      var assets = new T[guids.Length];
      for (var i = 0; i < guids.Length; i++)
      {
        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
        assets[i] = AssetDatabase.LoadAssetAtPath<T>(path);
      }

      return assets;
    }
  }
}