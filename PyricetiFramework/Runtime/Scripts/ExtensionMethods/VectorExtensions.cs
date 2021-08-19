using UnityEngine;

namespace PyricetiFramework
{
  public static class VectorExtensions
  {
    public static Vector3 OffsetX(this Vector3 v, float offsetX) => new Vector3(v.x + offsetX, v.y, v.z);
    public static Vector3 OffsetY(this Vector3 v, float offsetY) => new Vector3(v.x, v.y + offsetY, v.z);
    public static Vector3 OffsetZ(this Vector3 v, float offsetZ) => new Vector3(v.x, v.y, v.z + offsetZ);
  }
}