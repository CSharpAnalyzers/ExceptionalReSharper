using System;
using System.Security;

namespace Exceptional.Playground.Issues
{
    class DelegateIssues
    {
        public void Foo1()
        {
            Action x = delegate
            {
                throw new SecurityException(); // Optional warning
            };

            Action y = delegate
            {
                var a = 10;
                Bar(); // Optional warning
            };

            Action z = delegate
            {
                var b = new DelegateIssues(); // Issue: Should be optional warning  
            };
        }

        public void Foo2()
        {
            Action x = () =>
            {
                throw new SecurityException(); // Issue: Should be optional warning 
            };

            Action y = () =>
            {
                var a = 10;
                Bar(); // // Issue: Optional warning
            };

            Action z = () =>
            {
                var b = new DelegateIssues(); // Issue: Should be optional warning 
            };
        }

        public void Foo3()
        {
            Action y = Bar; // No warning
        }

        /// <exception cref="SecurityException">Condition. </exception>
        public void Bar()
        {
            throw new SecurityException();
        }

        /// <exception cref="SecurityException">Condition. </exception>
        public DelegateIssues()
        {
            throw new SecurityException();
        }
    }
}
