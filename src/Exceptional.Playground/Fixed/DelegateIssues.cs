using System;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    class DelegateIssues
    {
        public void Foo1()
        {
            Action x = delegate
            {
                throw new SecurityException(); // Hint
            };

            Action y = delegate
            {
                var a = 10;
                Bar(); // Hint
            };

            Action z = delegate
            {
                var b = new DelegateIssues(); // Hint  
            };
        }

        public void Foo2()
        {
            Action x = () =>
            {
                throw new SecurityException(); // Hint 
            };

            Action y = () =>
            {
                var a = 10;
                Bar(); // Hint
            };

            Action z = () =>
            {
                var b = new DelegateIssues(); // Hint 
            };
        }

        public void FooDirectCalls()
        {
            Bar(); // Warning
            var c = new DelegateIssues(); // Warning 
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
