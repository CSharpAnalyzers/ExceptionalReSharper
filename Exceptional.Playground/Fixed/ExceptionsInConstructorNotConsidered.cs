using System.IO;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    class ExceptionsInConstructorNotConsidered
    {
        public ExceptionsInConstructorNotConsidered()
        {
            var x = File.Open("abc", FileMode.Open); // Warning should be shown
            throw new SecurityException(); // Warning should be shown
        }
    }
}
