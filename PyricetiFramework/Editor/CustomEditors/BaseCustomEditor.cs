using System.Collections.Generic;
using UnityEditor;

namespace PyricetiFramework.Editor
{
  public abstract class BaseCustomEditor : UnityEditor.Editor
  {
    private List<string> customPropertiesNames;
    private Dictionary<string, SerializedProperty> customPropertiesByName;

    protected abstract List<string> GetCustomProperties();

    protected virtual void DrawDefaultProperty(SerializedProperty prop)
    {
      EditorGUILayout.PropertyField(prop, true);
    }
    
    protected abstract void DrawCustomProperty(string propName, SerializedProperty prop);

    protected virtual void OnEnable()
    {
      customPropertiesNames = GetCustomProperties() ?? new List<string>();
      customPropertiesByName = new Dictionary<string, SerializedProperty>();
      foreach (string propName in customPropertiesNames)
      {
        customPropertiesByName.Add(propName, serializedObject.FindProperty(propName));
      }
    }

    public override void OnInspectorGUI()
    {
      serializedObject.UpdateIfRequiredOrScript();

      SerializedProperty sp = serializedObject.GetIterator();
      for (var enterChildren = true; sp.NextVisible(enterChildren); enterChildren = false)
      {
        using (new EditorGUI.DisabledScope("m_Script" == sp.propertyPath))
        {
          if (!customPropertiesNames.Contains(sp.name))
            DrawDefaultProperty(sp);
          else
            DrawCustomProperty(sp.name, customPropertiesByName[sp.name]);
        }
      }
      
      serializedObject.ApplyModifiedProperties();
    }
  }
}