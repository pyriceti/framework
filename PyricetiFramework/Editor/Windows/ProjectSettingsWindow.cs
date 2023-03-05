using System;
using UnityEditor;
using UnityEngine;

namespace PyricetiFramework.Editor
{
  public class ProjectSettingsWindow : BaseWindow
  {
    private new const string EditorPrefsPrefix = BaseWindow.EditorPrefsPrefix + "projectSettingsWindow-";

    [MenuItem("PyricetiFramework/Project Settings Window", false, 11)]
    private static void Init() => InitImpl<ProjectSettingsWindow>("[PYR] Project Settings Window");

    private string projectSettingsPath;
    private ProjectSettings projectSettings;
    private UnityEditor.Editor projectSettingsEditor;

    protected override void OnEnable()
    {
      base.OnEnable();
      
      EngineConfigScriptableObject.ActiveInstanceUpdated += OnActiveInstanceUpdated;
      
      projectSettings = EngineConfigScriptableObject.GetActiveInstance<ProjectSettings>();
      if (projectSettings == null)
        return;
   
      projectSettingsEditor = UnityEditor.Editor.CreateEditor(projectSettings);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      
      EngineConfigScriptableObject.ActiveInstanceUpdated -= OnActiveInstanceUpdated;
    }

    private void OnActiveInstanceUpdated(Type type, EngineConfigScriptableObject instance)
    {
      if (type != typeof(ProjectSettings))
        return;

      DestroyImmediate(projectSettingsEditor);
      projectSettings = (ProjectSettings) instance;
      if (projectSettings == null)
        return;

      projectSettingsEditor = UnityEditor.Editor.CreateEditor(projectSettings);
    }

    protected override void DrawMainContent()
    {
      if (projectSettingsEditor != null && projectSettingsEditor.target != null)
      {
        if (GUILayout.Button("Focus source", GUILayout.ExpandWidth(false)))
          Selection.activeObject = projectSettings;
        projectSettingsEditor.OnInspectorGUI();
      }
      else
      {
        EditorGUILayout.LabelField(
          $"Cannot build editor for projectSettings (path: {projectSettingsPath}), please make sure the project is correctly setup in PyricetiFramework/Project Setup Window");
      }
    }
  }
}