using UnityEditor;
using UnityEngine;

namespace PyricetiFramework.Editor
{
  public class ProjectDataWindow : EditorWindow
  {
    private const string EditorPrefsPrefix = "pyricetiFramework-projectDataWindow-";
  
    #region Styles

    private const int MainPadding = 8;

    private GUIStyle cleanStyle;
    private GUIStyle mainScopeStyle;
    private GUIStyle mainScopePaddedStyle;
    private GUIStyle mainScrollViewStyle;
    private GUIStyle baseScrollViewStyle;
    private GUIStyle headerStyle;
    private GUIStyle foldoutStyle;
    private GUIStyle cleanButtonStyle;

    #endregion

    private float originalLabelWidthValue;
  
    private const string defaultProjectName = "SuperProject";
  
    private string projectName;

    [MenuItem("PyricetiFramework/Project Data Window")]
    private static void init()
    {
      var window = (ProjectDataWindow) GetWindow(typeof(ProjectDataWindow));
      window.titleContent = new GUIContent("[PYR] Project Data Window");
      window.Focus();
      window.Repaint();
      window.Show();
    }

    private void OnEnable()
    {
      loadEditorPrefs();
    }

    private void OnDisable()
    {
      saveEditorPrefs();
    }

    private void OnGUI()
    {
      initStyles();

      originalLabelWidthValue = EditorGUIUtility.labelWidth;
    
      using (new GUILayout.AreaScope(new Rect(0, 0, position.width, position.height), "", mainScopePaddedStyle))
      {
        drawMainContent();
        EditorGUIUtility.labelWidth = originalLabelWidthValue;
      }
    }

    private void initStyles()
    {
      cleanStyle = new GUIStyle
      {
        margin = new RectOffset(0, 0, 0, 0),
        padding = new RectOffset(0, 0, 0, 0),
        border = new RectOffset(0, 0, 0, 0),
      };

      mainScopeStyle = new GUIStyle(GUI.skin.window)
      {
        margin = new RectOffset(0, 0, 0, 0),
        padding = new RectOffset(0, 0, 0, 0),
        border = new RectOffset(0, 0, 0, 0),
        normal = new GUIStyleState { background = null }
      };

      mainScopePaddedStyle = new GUIStyle(mainScopeStyle)
      {
        padding = new RectOffset(MainPadding, MainPadding, MainPadding, MainPadding)
      };

      mainScrollViewStyle = new GUIStyle(GUI.skin.scrollView)
      {
        margin = new RectOffset(0, 0, 0, 0),
        padding = new RectOffset(MainPadding, MainPadding, MainPadding, MainPadding)
      };
    
      baseScrollViewStyle = new GUIStyle(GUI.skin.scrollView)
      {
        margin = new RectOffset(0, 0, 0, 0),
        padding = new RectOffset(0, 0, 0, 0),
        border = new RectOffset(0, 0, 0, 0),
      };

      headerStyle = new GUIStyle(EditorStyles.boldLabel);

      foldoutStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };

      cleanButtonStyle = new GUIStyle(GUI.skin.button) { margin = new RectOffset(0, 0, 0, 0) };
    }

 
    private void drawMainContent()
    {
      projectName = EditorGUILayout.TextField("Project Name", projectName);
      if (GUILayout.Button("Save", cleanButtonStyle, GUILayout.ExpandWidth(false)))
      {
        ProjectData.ProjectName = projectName;
      }
    }

    /// <summary>
    /// This is called on 10fps, so it ensures GUI is repainted often without performance issues
    /// </summary>
    public void OnInspectorUpdate() => Repaint();

    private void loadEditorPrefs()
    {
      projectName = EditorPrefs.GetString(EditorPrefsPrefix + nameof(projectName), defaultProjectName);
    }

    private void saveEditorPrefs()
    {
      EditorPrefs.SetString(EditorPrefsPrefix + nameof(projectName), projectName);
    }
  }
}