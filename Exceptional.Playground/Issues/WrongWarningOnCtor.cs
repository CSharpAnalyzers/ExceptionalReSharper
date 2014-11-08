using System.Security;

namespace Exceptional.Playground.Issues
{
    public class Foo : Bar
    {
        // No warning: Exception is thrown in base ctor
        /// <exception cref="SecurityException">Condition. </exception>
        public Foo()
        {
        }
    }

    public class Bar
    {
        /// <exception cref="SecurityException">Condition. </exception>
        public Bar()
        {
            throw new SecurityException();
        }
    }
}
