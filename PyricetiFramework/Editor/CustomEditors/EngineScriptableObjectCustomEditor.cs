using System.Collections.Generic;
using UnityEditor;

namespace PyricetiFramework.Editor
{
  [CustomEditor(typeof(EngineScriptableObject), true, isFallback = true)]
  [CanEditMultipleObjects]
  public class EngineScriptableObjectCustomEditor : BaseCustomEditor
  {
    protected override List<string> GetCustomProperties() => null;

    protected override void DrawCustomProperty(string propName, SerializedProperty prop) { }
  }
}