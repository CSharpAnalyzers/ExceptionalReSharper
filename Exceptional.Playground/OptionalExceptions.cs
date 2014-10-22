using System;
using System.Security;

namespace Exceptional.Playground
{
    public class OptionalExceptions
    {
        public void Foo()
        {
            Bar1(); // Hint: When "System.ArgumentException,InvocationOnly" is added to optional exceptions
            Bar2(); // Warning: When optional exceptions is default
        }

        /// <exception cref="ArgumentNullException">The input is wrong. </exception>
        public void Bar1()
        {
            throw new ArgumentNullException();
        }

        /// <exception cref="SecurityException">The user is not authenticated. </exception>
        public void Bar2()
        {
            throw new SecurityException();
        }
    }
}