using System;
using System.Security;

namespace Exceptional.Playground
{
    public class InnerExceptions
    {
        /// <exception cref="SecurityException">Sample. </exception>
        public void Test1()
        {
            try { }
            catch (ArgumentException exception)
            {
                throw new SecurityException(); // Warning: Inner exception not set
            }
        }

        /// <exception cref="SecurityException">Sample. </exception>
        public void Test2()
        {
            try { }
            catch (ArgumentException exception)
            {
                throw new SecurityException("Test", exception); // No warning: Inner exception is set
            }
        }
    }
}
