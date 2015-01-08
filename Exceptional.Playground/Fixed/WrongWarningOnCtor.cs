using System;
using System.IO;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class Foo : Bar
    {
        // Warning: Exception from base ctor not documented
        public Foo()
        {
        }

        // Warning on base call: Exception from base ctor not documented
        public Foo(int test)
            : base(test)
        {
        }

        // No warning: Exception is thrown in base ctor
        /// <exception cref="SecurityException">Condition. </exception>
        public Foo(string test)
            : base(test)
        {
        }

        // Warning: Exception is not thrown in base ctor
        /// <exception cref="SecurityException">Condition. </exception>
        public Foo(bool test)
            : base(test)
        {
        }
    }

    public class Bar
    {
        /// <exception cref="AccessViolationException">Condition. </exception>
        public Bar()
        {
            throw new AccessViolationException();
        }

        public Bar(bool test)
        {
        }

        /// <exception cref="IOException">Condition. </exception>
        public Bar(int test)
        {
            throw new IOException();
        }

        /// <exception cref="SecurityException">Condition. </exception>
        public Bar(string test)
        {
            throw new SecurityException();
        }
    }
}
