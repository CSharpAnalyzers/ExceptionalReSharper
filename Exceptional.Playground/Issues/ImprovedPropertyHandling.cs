using System.Runtime.Serialization;
using System.Security;

namespace Exceptional.Playground.Issues
{
    public class ImprovedPropertyHandling
    {
        public void Bar()
        {
            var x = Foo; // one warning (SecurityException)
            Foo = "test"; // one warning (SerializationException)

            var y = new ImprovedPropertyHandling();
            var b = y["abc"]; // one warning (SecurityException)
            y["abc"] = "h"; // one warning (SerializationException)
        }

        /// <exception cref="SecurityException" accessor="get">test</exception>
        /// <exception cref="SerializationException" accessor="set">abc</exception>
        public string Foo
        {
            get { throw new SecurityException("test"); }
            set { throw new SerializationException("abc"); }
        }

        /// <exception cref="SecurityException" accessor="get">test</exception>
        /// <exception cref="SerializationException" accessor="set">abc</exception>
        public string this[string index]
        {
            get { throw new SecurityException("test"); }
            set { throw new SerializationException("abc"); }
        }
    }
}