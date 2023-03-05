using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PyricetiFramework.Editor
{
  public class ProjectSetupWindow : BaseWindow
  {
    private new const string EditorPrefsPrefix = BaseWindow.EditorPrefsPrefix + "projectSetupWindow-";
    private const string DefaultDataPath = "Assets/Data";
    private const string DefaultProjectSettingsAssetName = "ProjectSettings.asset";

    [MenuItem("PyricetiFramework/Project Setup Window", false, 0)]
    private static void Init() => InitImpl<ProjectSetupWindow>("[PYR] Project Setup Window");

    [SerializeField] private string dataPath;

    private SerializedObject so;
    private SerializedProperty dataPathProp;
    
    private ProjectSettings projectSettings;

    protected override void OnEnable()
    {
      base.OnEnable();
      
      EngineConfigScriptableObject.ActiveInstanceUpdated += OnActiveInstanceUpdated;

      so = new SerializedObject(this);
      dataPathProp = so.FindProperty("dataPath");
      
      projectSettings = EngineConfigScriptableObject.GetActiveInstance<ProjectSettings>();
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

      if (instance != projectSettings)
        projectSettings = (ProjectSettings) instance;
    }

    protected override void DrawMainContent()
    {
      so.UpdateIfRequiredOrScript();
      
      EditorGUILayout.PropertyField(dataPathProp);

      EditorGUI.BeginChangeCheck();
      projectSettings =
        (ProjectSettings) EditorGUILayout.ObjectField("Project Settings", projectSettings, typeof(ProjectSettings),
          false);
      if (EditorGUI.EndChangeCheck())
      {
        projectSettings.SetActiveInstance();
      }

      if (GUILayout.Button("Generate ProjectData.asset file"))
      {
        
      }

      if (GUILayout.Button("Generate ProjectSettings.asset file"))
        GenerateProjectSettingsAsset();
      
      so.ApplyModifiedProperties();
    }

    private void GenerateProjectDataAsset()
    {
      
    }

    private void GenerateProjectSettingsAsset()
    {
      var newProjectSettings = CreateInstance<ProjectSettings>();

      string projectSettingsAssetPath = Path.Combine(dataPath, DefaultProjectSettingsAssetName);
      string directoryPath = Path.GetDirectoryName(projectSettingsAssetPath);
      Assert.IsNotNull(directoryPath);
      if (!Directory.Exists(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
        AssetDatabase.Refresh();
      }

      var existingProjectSettings = AssetDatabase.LoadAssetAtPath<ProjectSettings>(projectSettingsAssetPath);
      if (existingProjectSettings != null)
      {
        DestroyImmediate(newProjectSettings, true);
        newProjectSettings = existingProjectSettings;
        EditorUtility.DisplayDialog("ProjectSettings already exists", "A ProjectSettings.asset already exists!",
          "OK");
      }
      else
      {
        AssetDatabase.CreateAsset(newProjectSettings, projectSettingsAssetPath);
        AssetDatabase.SaveAssets();
      }

      EditorUtility.FocusProjectWindow();
      Selection.activeObject = newProjectSettings;
    }

    protected override void LoadEditorPrefs()
    {
      base.LoadEditorPrefs();

      dataPath = EditorPrefs.GetString(EditorPrefsPrefix + nameof(dataPath), DefaultDataPath);
    }

    protected override void SaveEditorPrefs()
    {
      base.SaveEditorPrefs();

      EditorPrefs.SetString(EditorPrefsPrefix + nameof(dataPath), dataPath);
    }
  }
}