using System.Collections.Generic;

namespace Exceptional.Playground.Fixed
{
    class AccessorOverrideTests
    {
        public void Foo()
        {
            var x = new Dictionary<string, string>();
            x["foo"] = "bar"; // No warning (disabled by predefined accessor override)
            var u = x["foo"]; // Warning
        }
    }
}
