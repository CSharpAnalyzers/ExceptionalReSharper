using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class ExceptionsOfCtorNotConsidered
    {
        public void Foo()
        {
            var test = new ExceptionsOfCtorNotConsidered2(); // Warning
        }
    }

    public class ExceptionsOfCtorNotConsidered2
    {
        /// <exception cref="SecurityException">Test</exception>
        public ExceptionsOfCtorNotConsidered2()
        {
            throw new SecurityException("Test");
        }
    }
}
