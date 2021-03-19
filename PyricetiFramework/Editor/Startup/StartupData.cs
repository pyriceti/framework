using System.Collections.Generic;
using UnityEngine;

namespace PyricetiFramework.Editor
{
  [CreateAssetMenu(fileName = "StartupData", menuName = ProjectData.ProjectName + "/StartupData", order = 1)]
  public class StartupData : ScriptableObject
  {
    [SerializeField] private SceneReference startupScene = default;
    [SerializeField] private List<SceneReference> mainScenes = default;

    #region Getters

    public SceneReference StartupScene => startupScene;

    public List<SceneReference> MainScenes => mainScenes;

    #endregion
  }
}