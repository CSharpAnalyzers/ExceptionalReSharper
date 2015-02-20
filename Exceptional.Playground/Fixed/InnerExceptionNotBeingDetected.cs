using System;

namespace Exceptional.Playground.Fixed
{
    class InnerExceptionNotBeingDetected
    {
        /// <exception cref="ArgumentException">Item could not be found.</exception>
        public void Foo1(string item)
        {
            try
            {
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentException("Item could not be found.", "item", ex); // Issue: "Include caught exception" is not working
            }
        }
    }
}
