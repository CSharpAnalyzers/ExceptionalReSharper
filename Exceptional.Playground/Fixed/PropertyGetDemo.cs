using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class PropertyGetDemo
    {
        public void Foobar()
        {
            var x = Foo; // Warning on property
        }

        // No warning
        /// <exception cref="SecurityException">Foo</exception>
        public string Foo
        {
            // No warning
            get { throw new SecurityException("Foo"); }
        }
    }
}