#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PyricetiFramework
{
  [CreateAssetMenu(fileName = "ProjectData", menuName = "PyricetiFramework/ProjectData", order = 1)]
  public class ProjectData : EngineConfigScriptableObject
  {
    public const string DefaultProjectName = "superjeu";

    [SerializeField] private string projectName = DefaultProjectName;

    public string ProjectName => this.projectName;
  }
}