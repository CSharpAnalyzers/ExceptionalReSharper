using System;

namespace Exceptional.Playground
{
    class ThrowingSystemExceptionShowsWarning
    {
        /// <exception cref="Exception">Test. </exception>
        public void Test03()
        {
            throw new Exception(); // Suggestion: Should show a suggestion because throwing System.Exception is considered bad. 
        }
    }
}
