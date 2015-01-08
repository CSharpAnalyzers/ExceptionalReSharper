using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class IndexerSetDemo
    {
        public void Foo()
        {
            var demo = new IndexerSetDemo();
            demo["foo"] = "foo"; // Warning on indexer
        }

        /// <exception cref="SecurityException">Foo</exception>
        public string this[string a]
        {
            set { throw new SecurityException("Foo"); }
        }
    }
}