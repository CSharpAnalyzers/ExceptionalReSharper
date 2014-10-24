using System;

namespace Exceptional.Playground
{
    class ThrowingSystemExceptionShowsWarning
    {
        /// <exception cref="Exception">Test. </exception>
        public void Test03()
        {
            throw new Exception(); // Should show a warning because throwing System.Exception is considered bad. 
        }
    }
}
