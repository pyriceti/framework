using UnityEngine;

namespace PyricetiFramework
{
  [CreateAssetMenu(fileName = "ProjectSettings", menuName = "PyricetiFramework/ProjectSettings", order = 1)]
  public class ProjectSettings : EngineConfigScriptableObject
  {
    [SerializeField] private bool isSerializedFieldCheckEnabled;

    public bool IsSerializedFieldCheckEnabled => this.isSerializedFieldCheckEnabled;
  }
}