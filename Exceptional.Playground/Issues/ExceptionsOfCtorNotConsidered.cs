using System.Security;

namespace Exceptional.Playground.Issues
{
    public class ExceptionsOfCtorNotConsidered
    {
        public void Foo()
        {
            var test = new ExceptionsOfCtorNotConsidered2(); // Should show warning
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
