using System.Security;

namespace Exceptional.Playground.Issues
{
    public class IndexerNotAnalyzed
    {
        public object this[int i]
        {
            get
            {
                throw new SecurityException("Foo"); // Warning
            }
        }

        /// <exception cref="SecurityException">Foo</exception>
        public object this[string i]
        {
            get
            {
                throw new SecurityException("Foo"); // No warning
            }
        }

        public void Foo()
        {
            var obj = this["Bar"]; // Warning (issue)
        }
    }
}
