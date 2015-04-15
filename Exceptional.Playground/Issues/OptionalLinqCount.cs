using System.Collections.Generic;
using System.Linq;

namespace Exceptional.Playground.Issues
{
    public class OptionalLinqCount
    {
        public void Foo()
        {
            var list = new List<int> { 1, 2 };
            var count = list.Count(); // Hint: No warning when OverflowException on Count is optional
        }
    }
}
