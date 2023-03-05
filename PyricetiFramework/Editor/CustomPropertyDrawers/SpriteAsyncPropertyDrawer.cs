using UnityEditor;
using UnityEngine;

namespace PyricetiFramework.Editor
{
  [CustomPropertyDrawer(typeof(SpriteAsync))]
  public class AssetAsyncPropertyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);
      Rect assetRefRect = new Rect(position.x, position.y, position.width, position.height);
      EditorGUI.PropertyField(assetRefRect, property.FindPropertyRelative("assetRef"), GUIContent.none);
      EditorGUI.EndProperty();
    }
  }
}