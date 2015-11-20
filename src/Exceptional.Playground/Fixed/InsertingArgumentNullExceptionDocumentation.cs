using System;

namespace Exceptional.Playground.Fixed
{
    class InsertingArgumentNullExceptionDocumentation
    {
        /// <exception cref="ArgumentNullException">foo is <see langword="null" />.</exception>
        public void Bar()
        {
            Foo(); // Should insert the same documentation. 
        }

        /// <exception cref="ArgumentNullException"><paramref name="foo"/> is <see langword="null" />.</exception>
        public void Foo()
        {
            throw new ArgumentNullException("foo"); // Should insert full documentation. 
        }
    }
}
