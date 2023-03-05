using System;
using UnityEditor;
using UnityEngine;


namespace PyricetiFramework.Editor
{
  /// <summary>
  /// TODO: add abstract sync/editable/resettable data window pattern w/ temp fields
  /// to improve perfs w/ lots of data
  /// TODO: Refactor w/ ProjectSettingsWindow
  /// </summary>
  public class ProjectDataWindow : BaseWindow
  {
    private new const string EditorPrefsPrefix = BaseWindow.EditorPrefsPrefix + "projectDataWindow-";

    private const string defaultProjectName = "SuperProject";

    [MenuItem("PyricetiFramework/Project Data Window", false, 11)]
    private static void Init() => InitImpl<ProjectDataWindow>("[PYR] Project Data Window");

    private string projectDataPath;
    private ProjectData projectData;
    private UnityEditor.Editor projectDataEditor;
    
    protected override void OnEnable()
    {
      base.OnEnable();
      
      EngineConfigScriptableObject.ActiveInstanceUpdated += OnActiveInstanceUpdated;
      
      projectData = EngineConfigScriptableObject.GetActiveInstance<ProjectData>();
      if (projectData == null)
        return;
   
      projectDataEditor = UnityEditor.Editor.CreateEditor(projectData);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      
      EngineConfigScriptableObject.ActiveInstanceUpdated -= OnActiveInstanceUpdated;
    }

    private void OnActiveInstanceUpdated(Type type, EngineConfigScriptableObject instance)
    {
      if (type != typeof(ProjectData))
        return;

      DestroyImmediate(projectDataEditor);
      projectData = (ProjectData) instance;
      if (projectData == null)
        return;

      projectDataEditor = UnityEditor.Editor.CreateEditor(projectData);
    }

    protected override void DrawMainContent()
    {
      if (projectDataEditor != null && projectDataEditor.target != null)
      {
        if (GUILayout.Button("Focus source", GUILayout.ExpandWidth(false)))
          Selection.activeObject = projectData;
        projectDataEditor.OnInspectorGUI();
      }
      else
      {
        EditorGUILayout.LabelField(
          $"Cannot build editor for projectData (path: {projectDataPath}), please make sure the project is correctly setup in PyricetiFramework/Project Setup Window");
      }
    }
  }
}