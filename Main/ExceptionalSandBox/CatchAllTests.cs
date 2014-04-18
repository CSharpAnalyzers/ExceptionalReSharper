using System;

namespace ExceptionalSandBox
{
    public class CatchAllTests
    {
        public void Test01()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch//BAD
            {
                Console.WriteLine("CATCH");
            }
        }

        public void Test02()
        {
            try { }
            catch (Exception)//BAD
            {
                Console.WriteLine("CATCH");
            }
        }

        public void Test03()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (Exception e)//BAD
            {
                Console.WriteLine("CATCH");
            }
        }

        public void Test04()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (ArgumentException e)//OK
            {
                Console.WriteLine("CATCH");
            }
        }
    }
}