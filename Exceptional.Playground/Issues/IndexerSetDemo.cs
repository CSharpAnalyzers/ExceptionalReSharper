using System.Security;

namespace Exceptional.Playground.Issues
{
    public class IndexerSetDemo
    {
        /// <exception cref="SecurityException">Foo</exception>
        public string this[string a]
        {
            set { throw new SecurityException("Foo"); }
        }

        public void Foo()
        {
            var demo = new IndexerSetDemo();
            demo["foo"] = "foo"; // Warning (issue)
        }
    }
}