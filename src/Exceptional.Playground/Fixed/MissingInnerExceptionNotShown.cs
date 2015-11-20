using System;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    // Issue priority: Low
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
                var exception = new Exception(); // Warning => Issue: When instantiating an exception in catch clause then "Missing inner exception" warning should be shown
                throw exception;
            }
        }
    }
}
