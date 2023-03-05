using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PyricetiFramework.Editor
{
  [CustomEditor(typeof(EngineObject), true, isFallback = true)]
  [CanEditMultipleObjects]
  public class EngineObjectCustomEditor : BaseCustomEditor
  {
    protected override List<string> GetCustomProperties() => null;

    protected override void DrawDefaultProperty(SerializedProperty prop)
    {
      var projectSettings = EngineConfigScriptableObject.GetActiveInstance<ProjectSettings>();

      if (projectSettings != null && !projectSettings.IsSerializedFieldCheckEnabled)
      {
        base.DrawDefaultProperty(prop);
        return;
      }

      // Check for unset component serialized references
      if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null)
      {
        FieldInfo fieldInfo = serializedObject.targetObject.GetType().GetField(prop.name,
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        if (fieldInfo != null && fieldInfo.GetCustomAttribute<MayBeNullOnAwake>() == null)
        {
          Color defaultBgColor = GUI.color;
          GUI.color = Colors.ErrorColor;
          EditorGUILayout.PropertyField(prop,
            new GUIContent($"{ObjectNames.NicifyVariableName(prop.name)} (Missing)"),
            true);
          GUI.color = defaultBgColor;
        }
        else
          base.DrawDefaultProperty(prop);
      }
      else
        base.DrawDefaultProperty(prop);
    }

    protected override void DrawCustomProperty(string propName, SerializedProperty prop) { }
  }
}