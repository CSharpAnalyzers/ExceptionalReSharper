using System;
using System.Security;

namespace Exceptional.Playground.Issues
{
    class MissingInnerExceptionNotShown
    {
        /// <exception cref="Exception">Condition. </exception>
        public void Foo()
        {
            try
            {
                throw new SecurityException();
            }
            catch (SecurityException securityException)
            {
                var exception = new Exception(); // Issue: When instantiating an exception in catch clause then "Missing inner exception" warning should be shown
                throw exception;
            }
        }
    }
}
