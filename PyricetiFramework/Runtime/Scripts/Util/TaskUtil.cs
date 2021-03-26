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
  }
}