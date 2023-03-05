using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PyricetiFramework.Editor
{
  [CustomEditor(typeof(EngineConfigScriptableObject), true, isFallback = true)]
  [CanEditMultipleObjects]
  public class EngineConfigScriptableObjectCustomEditor : EngineScriptableObjectCustomEditor
  {
    protected override List<string> GetCustomProperties()
    {
      return new List<string> { "isTheActiveInstance" };
    }

    protected override void DrawCustomProperty(string propName, SerializedProperty prop)
    {
      switch (propName)
      {
        case "isTheActiveInstance":
          if (prop.boolValue)
          {
            Color defaultBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Colors.InfoColor;
            EditorGUILayout.PropertyField(prop);
            GUI.backgroundColor = defaultBgColor;  
          }
          else
          {
            EditorGUILayout.PropertyField(prop);
          }
          break;
      }
    }
  }
}