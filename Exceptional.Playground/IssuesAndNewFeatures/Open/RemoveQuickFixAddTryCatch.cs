using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptional.Playground.IssuesAndNewFeatures.Open
{
  // Optional: To discuss first

  class RemoveQuickFixAddTryCatch
  {
    public void Test1()
    {
      // Is quick fix "Catch exception ..." really necessary => it creates a try around single statement which is most likely not desired...
      // my suggestion: remove quick fix "Catch exception ..." completely with no replacement (can be done manually)

      throw new ArgumentException(); // Should only show "Add documentation ..." but not "Catch exception ..."
    }
  }
}
