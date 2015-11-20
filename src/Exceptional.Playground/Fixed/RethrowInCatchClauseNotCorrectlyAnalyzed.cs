using System.Diagnostics;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    class RethrowInCatchClauseNotCorrectlyAnalyzed
    {
        public void Foo()
        {
            try
            {
                throw new SecurityException();
            }
            catch (SecurityException exception)
            {
                Debug.WriteLine(exception.Message);
                throw exception; // Warning: This is correctly shown as warning
            }
        }

        public void Bar()
        {
            try
            {
                throw new SecurityException();
            }
            catch (SecurityException exception)
            {
                Debug.WriteLine(exception.Message);
                throw; // Warning: SecurityException is not documented
            }
        }
    }
}
