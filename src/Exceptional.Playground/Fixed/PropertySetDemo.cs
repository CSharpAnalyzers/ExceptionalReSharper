using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class PropertySetDemo
    {
        public void Foobar()
        {
            Foo = "test"; // Warning on property
        }

        // No warning
        /// <exception cref="SecurityException">Foo</exception>
        public string Foo
        {
            // No warning
            set { throw new SecurityException("Foo"); }
        }
    }
}
