using System;

namespace PyricetiFramework
{
  public static class ArrayExtensions
  {
    /// <summary>
    /// Taken from https://stackoverflow.com/a/457501/8658784
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[] RemoveAt<T>(this T[] source, int index)
    {
      var dest = new T[source.Length - 1];
      if (index > 0)
        Array.Copy(source, 0, dest, 0, index);

      if (index < source.Length - 1)
        Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

      return dest;
    }
  }
}