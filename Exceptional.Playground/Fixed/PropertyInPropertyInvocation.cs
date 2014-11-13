using System.Security;

namespace Exceptional.Playground.Fixed
{
    class PropertyInPropertyInvocation
    {
        public string Bar
        {
            get
            {
                return Foo; // Warning
            }
        }

        /// <exception cref="SecurityException">Hello</exception>
        public string Foo
        {
            get
            {
                throw new SecurityException("Hello");
            }
        }
    }
}
