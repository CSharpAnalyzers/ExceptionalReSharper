using System.Security;

namespace Exceptional.Playground
{
    public class UnnecessaryExceptionDocumented
    {
        // Warning: Exception is not thrown but documented

        /// <summary>Abc. </summary>
        /// <exception cref="SecurityException">Sample. </exception>
        public void Test2()
        {
        }
    }
}
