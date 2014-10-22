using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Exceptional.Playground
{
    public class OptionalMethodExceptions
    {
        public void Foo()
        {
            IDictionary dictionary = new Dictionary<string, string>();
            dictionary.Add("a", "b"); // Hint: When optional method exceptions is default

            File.Open("abc", FileMode.Open); // Warning: When optional method exceptions is default
        }
    }
}