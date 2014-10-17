using System;

namespace Exceptional.Playground.IssuesAndNewFeatures.Fixed
{
  public class Problem02
  {
    public void Test1()
    {
      throw new ArgumentException(); // Bug: Add documentation crashes
    }

    /// <summary>Test. </summary>
    public void Test2()
    {
      
    }
  }
}
