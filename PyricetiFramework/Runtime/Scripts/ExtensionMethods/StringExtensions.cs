namespace PyricetiFramework
{
  public static class StringExtensions
  {
    /// <summary>
    /// Better performance than usual string.StartsWith
    /// see https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CustomStartsWith(this string a, string b)
    {
      int aLen = a.Length;
      int bLen = b.Length;

      var ap = 0;
      var bp = 0;

      while (ap < aLen && bp < bLen && a[ap] == b[bp])
      {
        ap++;
        bp++;
      }

      return (bp == bLen);
    }

    /// <summary>
    /// Better performance than usual string.EndsWith
    /// see https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CustomEndsWith(this string a, string b)
    {
      int ap = a.Length - 1;
      int bp = b.Length - 1;

      while (ap >= 0 && bp >= 0 && a[ap] == b[bp])
      {
        ap--;
        bp--;
      }

      return (bp < 0);
    }
  }
}