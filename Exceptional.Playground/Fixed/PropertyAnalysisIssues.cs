using System.Security;

namespace Exceptional.Playground.Fixed
{
    class PropertyAnalysisIssues
    {
        // No warning 
        /// <exception cref="SecurityException">Foo</exception>
        public string Foo
        {
            // No warning
            get { throw new SecurityException("Foo"); }
        }

        // No warning 
        /// <exception cref="SecurityException">Bar</exception>
        public string Bar
        {
            // No warning
            set { throw new SecurityException("Bar"); }
        }

        public string Foo2
        {
            // Warning
            get { throw new SecurityException("Foo2"); }
        }

        public string Bar2
        {
            // Warning
            set { throw new SecurityException("Bar2"); }
        }

        // No warning
        /// <exception cref="SecurityException">Foobar</exception>
        public void Foobar()
        {
            // No warning
            throw new SecurityException("Foobar");
        }
    }
}
