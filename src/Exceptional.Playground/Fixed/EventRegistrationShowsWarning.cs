using System;
using System.Linq.Expressions;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    class EventRegistrationShowsWarning
    {
        public void Foo()
        {
            Test1(Xyz); // No warning: Not called here so no warning should be shown

            MyEvent += Bar; // No warning: Not called here so no warning should be shown
            
            var copy = MyEvent;
            if (copy != null)
                copy(); // Suggestion: May throw exception

            var value = 10;
            value += Xyz(); // Warning: SecurityException not documented

            Test2(Xyz()); // Warning: SecurityException not documented
            Test2(value); // No warning

            Xyz(); // Warning
            var x = Xyz(); // Warning
            x = Xyz() + Xyz(); // Two warnings
            x = Xyz() + 10 + Xyz(); // Two warnings
        }

        public event Action MyEvent;

        /// <exception cref="SecurityException">Condition. </exception>
        public void Bar()
        {
            throw new SecurityException();
        }

        /// <exception cref="SecurityException">Condition. </exception>
        public int Xyz()
        {
            throw new SecurityException();
            return 42;
        }

        public void Test1(Func<int> func)
        {
        }

        public void Test2(int val)
        {
        }
    }
}
