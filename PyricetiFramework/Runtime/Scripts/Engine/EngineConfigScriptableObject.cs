using System;
using System.Collections.Generic;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using UnityEngine;

namespace PyricetiFramework
{
  /// <summary>
  /// TODO: profile to check what is time consuming when recompiling scripts
  /// </summary>
  public class EngineConfigScriptableObject : ScriptableObject
  {
    [ReadOnly] [SerializeField] protected bool isTheActiveInstance;

    public bool IsTheActiveInstance => isTheActiveInstance;

    public static event Action<Type, EngineConfigScriptableObject> ActiveInstanceUpdated;
    
    private static readonly Dictionary<Type, EngineConfigScriptableObject> ActiveInstances = new();

    public static T GetActiveInstance<T>() where T : EngineConfigScriptableObject
    {
      if (ActiveInstances.ContainsKey(typeof(T)))
        return (T) ActiveInstances[typeof(T)];
      return null;
    }

    [UsedImplicitly]
    public static bool IsActiveInstance<T>() where T : EngineConfigScriptableObject =>
      ActiveInstances.ContainsKey(typeof(T));

    [UsedImplicitly]
    public static void RegisterActiveInstance<T>(EngineConfigScriptableObject instance) where T : EngineConfigScriptableObject
    {
      if (!ActiveInstances.ContainsKey(typeof(T)))
        ActiveInstances.Add(typeof(T), instance);
      else
        ActiveInstances[typeof(T)] = instance;
    }

    [UsedImplicitly]
    public static void LoadActiveInstanceFromAssets<T>(EngineConfigScriptableObject instanceToIgnore = null)
      where T : EngineConfigScriptableObject
    {
      T[] assets = AssetUtil.GetAllInstances<T>();
      if (assets == null || assets.Length <= 0)
        return;
      
      foreach (T a in assets)
      {
        if (a == instanceToIgnore)
          continue;
        
        a.SetActiveInstance();
        return;
      }
    }

    [UsedImplicitly]
    protected bool IsActiveInstance()
    {
      if (!ActiveInstances.ContainsKey(GetType()))
        return false;
      return ActiveInstances[GetType()] == this;
    }

#if UNITY_EDITOR
    [ContextMenu("Set as active instance")]
    private void cm_setActiveInstance()
    {
      SetActiveInstance();
    }

    public void SetActiveInstance()
    {
      if (this == null)
        return;
      
      GetType()
        .GetMethod("RegisterActiveInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        ?.MakeGenericMethod(GetType())
        .Invoke(this, new object[] { this });

      isTheActiveInstance = true;
      EditorUtility.SetDirty(this);
      // AssetDatabase.SaveAssets();
      
      ActiveInstanceUpdated?.Invoke(GetType(), this);
    }

    private void UnsetActiveInstance()
    {
      if (this == null)
        return;
      
      isTheActiveInstance = false;
      EditorUtility.SetDirty(this);
      // AssetDatabase.SaveAssets();
    }

    private void OnEnable()
    {
      ActiveInstanceUpdated += OnActiveInstanceUpdated;
      RefreshActiveInstanceState();
    }

    private void OnDisable()
    {
      ActiveInstanceUpdated -= OnActiveInstanceUpdated;
    }
    
    private void OnActiveInstanceUpdated(Type type, EngineConfigScriptableObject instance)
    {
      if (GetType() != type)
        return;

      if (this != instance)
        UnsetActiveInstance();
    }

    private void RefreshActiveInstanceState()
    {
      object rawIsRegisteredActiveInstance = GetType()
        .GetMethod("IsActiveInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        ?.MakeGenericMethod(GetType())
        .Invoke(null, null);

      bool isActiveInstanceOfThisTypeRegistered = rawIsRegisteredActiveInstance != null && (bool) rawIsRegisteredActiveInstance;
      
      object rawActiveInstanceOfThisTypeRegistered = GetType()
        .GetMethod("GetActiveInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        ?.MakeGenericMethod(GetType())
        .Invoke(null, null);

      bool isValidActiveInstanceOfThisTypeRegistered =
        isActiveInstanceOfThisTypeRegistered && rawActiveInstanceOfThisTypeRegistered != null;

      if (isValidActiveInstanceOfThisTypeRegistered)
      {
        if (isTheActiveInstance)
        {
          if (ReferenceEquals(this, rawActiveInstanceOfThisTypeRegistered))
            SetActiveInstance();
          else
            UnsetActiveInstance();
        }
        else
          UnsetActiveInstance();
      }
      else
      {
        SetActiveInstance();
      }
    }
#endif
  }

#if UNITY_EDITOR
  public class EngineConfigScriptableObjectDeleteDetector : AssetModificationProcessor
  {
    private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
    {
      if (!AssetDatabase.GetMainAssetTypeAtPath(path)?.IsSubclassOf(typeof(EngineConfigScriptableObject)) ?? true)
        return AssetDeleteResult.DidNotDelete;
      
      var asset = AssetDatabase.LoadAssetAtPath<EngineConfigScriptableObject>(path);
      if (asset != null && asset.IsTheActiveInstance)
      {
        asset.GetType()
          .GetMethod("LoadActiveInstanceFromAssets",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
          ?.MakeGenericMethod(asset.GetType())
          .Invoke(null, new object[] { asset });
      }

      return AssetDeleteResult.DidNotDelete;
    }
  }
#endif
}