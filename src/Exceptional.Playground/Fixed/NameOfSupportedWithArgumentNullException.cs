namespace Exceptional.Playground.Fixed
{
    using System;

    internal class NameOfSupportedWithArgumentNullException
    {
        public void Foo(object bar)
        {
            if (bar == null)
            {
                // Alt+Enter -> Add documentation 
                // should generate correct paramref name "bar"
                throw new ArgumentNullException(nameof(bar));                    
            }
        }

        public void Foo2(object bar)
        {
            if (bar == null)
            {
                throw new ArgumentNullException("some neat message", (Exception)null);
            }
        }

        public void Foo3(object bar)
        {
            if (bar == null)
            {
                throw new ArgumentNullException();
            }
        }

        public void Foo4(object bar)
        {
            if (bar == null)
            {
                throw new ArgumentNullException("bar", "message");
            }
        }

        public void Foo5(object bar)
        {
            if (bar == null)
            {
                throw new ArgumentNullException(nameof(bar), "message");
            }
        }
    }
}