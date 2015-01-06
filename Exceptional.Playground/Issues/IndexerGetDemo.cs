using System.Security;

namespace Exceptional.Playground.Issues
{
    public class IndexerGetDemo
    {
        /// <exception cref="SecurityException">Foo</exception>
        public string this[string a]
        {
            get { throw new SecurityException("Foo"); }
        }

        public void Foo()
        {
            var demo = new IndexerGetDemo();
            var bar = demo["foo"]; // Warning (issue)
        }
    }
}