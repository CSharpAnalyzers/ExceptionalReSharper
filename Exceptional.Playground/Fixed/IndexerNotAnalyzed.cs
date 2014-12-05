using System.Security;

namespace Exceptional.Playground.Fixed
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
    }
}
