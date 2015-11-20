using System.Runtime.Serialization;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class PropertyAccessorSpecificExceptions
    {
        public void Bar()
        {
            var x = Foo; // one warning (SecurityException)
            Foo = "test"; // one warning (SerializationException)

            var y = new PropertyAccessorSpecificExceptions();
            var b = y["abc"]; // one warning (SecurityException)
            y["abc"] = "h"; // one warning (SerializationException)

            var z = Blu; // two warnings
            Blu = "test"; // two warnings
        }

        /// <exception cref="SecurityException" accessor="get">test</exception>
        /// <exception cref="SerializationException" accessor="set">abc</exception>
        public string Foo
        {
            get { throw new SecurityException("test"); }
            set { throw new SerializationException("abc"); }
        }

        /// <exception cref="SecurityException">test</exception>
        /// <exception cref="SerializationException">abc</exception>
        public string Blu
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

        // Warning - Ok - Warning
        /// <exception cref="SerializationException" accessor="set">test</exception>
        /// <exception cref="SerializationException" accessor="get">test</exception>
        /// <exception cref="SecurityException" accessor="set">test</exception>
        public string Test
        {
            get { throw new SerializationException("test"); }
        }
    }
}