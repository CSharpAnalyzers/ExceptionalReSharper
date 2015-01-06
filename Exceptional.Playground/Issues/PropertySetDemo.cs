using System.Security;

namespace Exceptional.Playground.Issues
{
    public class PropertySetDemo
    {
        // No warning
        /// <exception cref="SecurityException">Foo</exception>
        public string Foo
        {
            // No warning
            set { throw new SecurityException("Foo"); }
        }

        public void Foobar()
        {
            Foo = "test"; // Warning (issue)
        }
    }
}
