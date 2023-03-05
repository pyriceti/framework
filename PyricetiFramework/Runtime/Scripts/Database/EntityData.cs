using System;
using JetBrains.Annotations;
using PyricetiFramework;
using UnityEngine;
using UnityEditor;

/// <summary>
/// TODO: Create custom inspector, especially to check for non-empty unique Ids
/// </summary>
public abstract class EntityData : EngineScriptableObject
{
  private const string nonInitializedGuid = "00000000000000000000000000000000";

  [ReadOnly] [SerializeField] private string guid;

  [field: SerializeField, Tooltip("Human-readable, must be unique among entities of same type")]
  public string Id { get; [UsedImplicitly] private set; }

  public string Guid => this.guid;

  [ContextMenu("Update guid")]
  public string UpdateGuid()
  {
    string oldGuid = this.guid;
    bool success = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out this.guid, out long _);
    if (success)
      return this.guid;

    this.guid = oldGuid;
    return this.guid;
  }

  public string UpdateGuidIFN()
  {
    if (string.IsNullOrEmpty(this.guid) || this.guid == nonInitializedGuid)
      return this.UpdateGuid();

    return this.guid;
  }

  private void OnEnable() => this.UpdateGuidIFN();

  private void Awake() => this.UpdateGuidIFN();
}