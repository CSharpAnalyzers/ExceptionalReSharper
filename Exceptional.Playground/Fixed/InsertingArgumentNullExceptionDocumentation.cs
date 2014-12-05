using System;

namespace Exceptional.Playground.Fixed
{
    class InsertingArgumentNullExceptionDocumentation
    {
        /// <exception cref="ArgumentNullException">The value of 'foo' cannot be null. </exception>
        public void Bar()
        {
            Foo(); // Should insert the same documentation. 
        }

        /// <exception cref="ArgumentNullException">The value of 'foo' cannot be null. </exception>
        public void Foo()
        {
            throw new ArgumentNullException("foo"); // Should insert full documentation. 
        }
    }
}
