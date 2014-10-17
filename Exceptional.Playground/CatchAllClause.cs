using System;
using System.Security;

namespace Exceptional.Playground
{
    public class CatchAllClause
    {
        public void Test01()
        {
            try
            {
                throw new SecurityException();
            }
            catch (Exception exception) // Warning: When catching System.Exception a warning should be shown
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void Test02()
        {
            try
            {
                throw new SecurityException();
            }
            catch (Exception exception) // Tow warnings: Same as above and one from ReSharper
            {
            }
        }
    }
}
