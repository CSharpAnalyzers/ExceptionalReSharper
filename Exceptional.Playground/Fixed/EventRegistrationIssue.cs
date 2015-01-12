using System;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class EventRegistrationIssue
    {
        public void Foo()
        {
            MyEvent += Bar; // No warning 
            MyEvent -= Bar; // No warning 
            MyEvent = Bar; // No warning 

            Bar(); // Warning
        }

        /// <exception cref="SecurityException">Bar</exception>
        private void Bar()
        {
            throw new SecurityException("Bar");
        }

        public event Action MyEvent;
    }
}
