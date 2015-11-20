using System.Security;

namespace Exceptional.Playground.Fixed
{
    public interface IExceptionOnInterfaceMarkedAsUnnecessary
    {
        // No warning
        /// <exception cref="SecurityException">Test. </exception>
        void Test();
    }

    public abstract class ExceptionOnInterfaceMarkedAsUnnecessary
    {
        // No warning
        /// <exception cref="SecurityException">Test. </exception>
        public abstract void Test();

        // Warning
        /// <exception cref="SecurityException">Test. </exception>
        void Test02()
        {
        }
    }
}
