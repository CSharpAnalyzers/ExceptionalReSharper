using System;

namespace Exceptional.Playground.IssuesAndNewFeatures.Open
{
  // Optional: To discuss first

  public class HideWarningWhenBaseExceptionAlreadyDocumented
  {
    /// <exception cref="Exception">Test. </exception>
    public void Test2(int i)
    {
      if (i == 0)
        throw new ArgumentException(); // Should this be hidden because base exception (System.Exception) is already documented?
      throw new Exception();
    }

    /// <exception cref="Exception">Test. </exception>
    public void Test03()
    {
      throw new Exception(); // Should still be a warning because throwing System.Exception is considered bad. 
    }
  }
}
