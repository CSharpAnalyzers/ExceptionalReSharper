using System.Security;

namespace Exceptional.Playground.Issues
{
    public class PropertyGetDemo
    {
        // No warning
        /// <exception cref="SecurityException">Foo</exception>
        public string Foo
        {
            // No warning
            get { throw new SecurityException("Foo"); }
        }

        public void Foobar()
        {
            var x = Foo; // Warning
        }
    }
}