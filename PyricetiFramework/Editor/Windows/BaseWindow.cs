using UnityEditor;
using UnityEngine;

namespace PyricetiFramework.Editor
{
  public abstract class BaseWindow : EditorWindow
  {
    protected const string EditorPrefsPrefix = "pyricetiFramework-";
  
    #region Styles

    private const int MainPadding = 8;

    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable NotAccessedField.Global
    protected GUIStyle cleanStyle;
    protected GUIStyle mainScopeStyle;
    protected GUIStyle mainScopePaddedStyle;
    protected GUIStyle mainScrollViewStyle;
    protected GUIStyle baseScrollViewStyle;
    protected GUIStyle headerStyle;
    protected GUIStyle foldoutStyle;
    protected GUIStyle cleanButtonStyle;
    // ReSharper restore NotAccessedField.Global
    // ReSharper restore MemberCanBePrivate.Global

    #endregion

    private float originalLabelWidthValue = -1f;

    public float OriginalLabelWidthValue =>
      originalLabelWidthValue < 0 ? EditorGUIUtility.labelWidth : originalLabelWidthValue;

    /// <summary>
    /// This method should be called in an "init-like" method with the [MenuItem] decorator.
    /// </summary>
    /// <param name="windowTitle"></param>
    /// <typeparam name="T"></typeparam>
    protected static void InitImpl<T>(string windowTitle) where T : EditorWindow
    {
      var window = (T) GetWindow(typeof(T));
      window.titleContent = new GUIContent(windowTitle);
      window.Focus();
      window.Repaint();
      window.Show();
    }

    protected virtual void OnEnable()
    {
      LoadEditorPrefs();
    }

    protected virtual void OnDisable()
    {
      SaveEditorPrefs();
    }

    private void OnGUI()
    {
      InitRefs();
      InitBaseStyles();
      InitStyles();

      originalLabelWidthValue = EditorGUIUtility.labelWidth;
    
      using (new GUILayout.AreaScope(new Rect(0, 0, position.width, position.height), "", mainScopePaddedStyle))
      {
        DrawMainContent();
        EditorGUIUtility.labelWidth = originalLabelWidthValue;
      }
    }

    private void InitBaseStyles()
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
    
    protected virtual void InitRefs() { }
    
    protected virtual void InitStyles() { }

    protected abstract void DrawMainContent();

    /// <summary>
    /// This is called on 10fps, so it ensures GUI is repainted often without performance issues
    /// </summary>
    public void OnInspectorUpdate() => Repaint();

    protected virtual void LoadEditorPrefs() { }

    protected virtual void SaveEditorPrefs() { }
  }
}