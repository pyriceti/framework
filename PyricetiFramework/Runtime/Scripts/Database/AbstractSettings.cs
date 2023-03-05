using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PyricetiFramework
{
  public abstract class AbstractSettings : EngineScriptableObject
  {
#if UNITY_EDITOR
    private static string SettingsLabel =>
      $"{EngineConfigScriptableObject.GetActiveInstance<ProjectData>()?.ProjectName ?? ProjectData.DefaultProjectName}Settings";

    private void OnEnable()
    {
      string[] currentLabels = AssetDatabase.GetLabels(this);
      if (currentLabels.Contains(SettingsLabel))
        return;
      var newLabels = new List<string>(currentLabels) { SettingsLabel };
      AssetDatabase.SetLabels(this, newLabels.ToArray());
      EditorUtility.SetDirty(this);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    [ContextMenu("Reset to non debug settings")]
    private void ResetToNonDebugSettings()
    {
      if (!this.name.CustomEndsWith("Debug"))
      {
        Debug.LogWarning("Cannot reset none debug settings, aborting");
        return;
      }

      string debugSettingsPath = AssetDatabase.GetAssetPath(this);
      int suffixLength = "Debug.asset".Length;
      string settingsPath = debugSettingsPath[..^suffixLength] + ".asset";

      var settings = AssetDatabase.LoadAssetAtPath<AbstractSettings>(settingsPath);
      if (settings == null)
      {
        Debug.LogWarning($"Cannot find {settingsPath}, aborting");
        return;
      }

      Debug.Log($"Copying {settings.name} values to {this.name}");

      EditorUtility.CopySerialized(settings, this);
      EditorUtility.SetDirty(this);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }
#endif
  }
}