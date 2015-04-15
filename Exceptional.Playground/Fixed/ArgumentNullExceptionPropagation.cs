using System;

namespace Exceptional.Playground.Fixed
{
    class ArgumentNullExceptionPropagation
    {
        /// <exception cref="ArgumentNullException">abc is <see langword="null" />.</exception>
        public void Foo()
        {
            Bar(null); // Should insert "abc is <see langword="null" />." instead of "<paramref name="abc"/> is ..."
        }

        /// <exception cref="ArgumentNullException"><paramref name="abc"/> is <see langword="null" />.</exception>
        public void Bar(string abc)
        {
            throw new ArgumentNullException("abc");
        }
    }
}
