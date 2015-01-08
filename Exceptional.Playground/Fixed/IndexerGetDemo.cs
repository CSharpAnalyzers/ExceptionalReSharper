using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class IndexerGetDemo
    {
        public void Foo()
        {
            var demo = new IndexerGetDemo();
            var bar = demo["foo"]; // Warning on indexer
        }
        
        /// <exception cref="SecurityException">Foo</exception>
        public string this[string a]
        {
            get { throw new SecurityException("Foo"); }
        }
    }
}