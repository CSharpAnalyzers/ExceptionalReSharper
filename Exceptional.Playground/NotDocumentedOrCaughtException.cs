using System.Security;

namespace Exceptional.Playground
{
    public class NotDocumentedOrCaughtException
    {
        public void Test1()
        {
            throw new SecurityException(); // Warning: Exception is not documented. 
        }

        public void Test2()
        {
            try
            {
                throw new SecurityException(); // No warning: Exception is caught.  
            }
            catch (SecurityException securityException)
            {
            }
        }

        /// <exception cref="SecurityException">Sample. </exception>
        public void Test3()
        {
            throw new SecurityException(); // No warning: Exception is documented.  
        }
    }
}
