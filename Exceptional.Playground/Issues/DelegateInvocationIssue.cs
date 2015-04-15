using System;
using System.Security;

namespace Exceptional.Playground.Issues
{
    public class DelegateInvocationIssue
    {
        public void Foo(Action foo)
        {
            _myAction = foo; // No suggestions 

            this["foo"] = null; // No suggestion
            this["bar"](); // Suggestion => Issue: Suggestion not shown
        }

        private Action _myAction;

        /// <exception cref="SecurityException">Bar</exception>
        public Action this[string i]
        {
            get { throw new SecurityException("Bar"); }
            set { throw new SecurityException("Bar"); }
        }
    }
}
