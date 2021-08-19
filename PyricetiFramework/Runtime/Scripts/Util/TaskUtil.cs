using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace PyricetiFramework
{
  [SuppressMessage("ReSharper", "UnusedType.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public static class TaskUtil
  {
    public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource)
    {
      tokenSource?.Cancel();
      tokenSource?.Dispose();
      tokenSource = new CancellationTokenSource();
      return tokenSource.Token;
    }
    
    /// <summary>
    /// Allow to automatically add/remove the cts to/from the provided cts list
    /// </summary>
    /// <param name="tokenSource"></param>
    /// <param name="ctsList"></param>
    /// <returns></returns>
    public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource,
      List<CancellationTokenSource> ctsList)
    {
      if (tokenSource != null)
      {
        tokenSource.Cancel();
        tokenSource.Dispose();
        ctsList.Remove(tokenSource);
      }

      tokenSource = new CancellationTokenSource();
      ctsList.Add(tokenSource);
      return tokenSource.Token;
    }
  }
}