using System;

namespace ExceptionalSandBox
{
    public class SandBox
    {
        public void Test01() {} //OK

        public void Test02()
        {
            throw new InvalidOperationException();//BAD
        }

        /// <summary>This method...</summary>
        public void Test03()
        {
            throw new InvalidOperationException();//BAD
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when....</exception>
        public void Test04()
        {
            throw new InvalidOperationException();//OK
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when....</exception>
        public void Test05()
        {
            var ex = new InvalidOperationException();
            throw ex;//OK
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when....</exception>//BAD
        public void Test06()
        {
            Exception ex = new InvalidOperationException();
            throw ex;//BAD
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when...</exception> //BAD
        public void Test07()
        {
            //look I'm not throwing
        }

        public void Test08()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            //Swallowing excepions is not recommended.
            catch (Exception)//BAD
            {
                Console.WriteLine("CATCH");
            }
        }

        /// <exception cref="InvalidOperationException">Test</exception>
        public void Test09()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (Exception ex)//OK - check if it is thrown as inner exception or rethrown
            {
                throw new InvalidOperationException("Test", ex);
            }
        }

        public void Test10()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (OperationCanceledException ex)
            {
                try
                {
                    throw new InvalidOperationException("Test", ex);//OK
                }
                catch (InvalidOperationException) { }
            }
        }

        //public void Test11()
        //{
        //    try
        //    {
        //        Console.WriteLine("TRY");
        //    }
        //    catch
        //    {
        //        Console.WriteLine("TRYs");
        //    }
        //}

        //public void Test12()
        //{
        //    try
        //    {
        //        Console.WriteLine("TRY");
        //    }
        //    catch(InvalidOperationException)
        //    {
                
        //    }
        //    catch(Exception)
        //    {
        //        Console.WriteLine("TRYs");
        //    }
        //}

//        /// <exception cref="InvalidOperationException">sss</exception>
//        public void Test13()
//        {
//            try
//            {
//                throw new OperationCanceledException();
//            }
//            //All ecxeptions thrown from inside of catch clause that do not 
//            //have a variable defined are reported as missing inner exception.
//            catch (OperationCanceledException)
//            {
//                //We should include a catched exception as inner exception
//                throw new InvalidOperationException("sss");
//            }
//        }
//
//        /// <exception cref="InvalidOperationException">sss</exception>
//        public void Test14(bool flag, bool flag2)
//        {
//            try
//            {
//                Console.WriteLine();
//            }
//            //All ecxeptions thrown from inside of catch clause that do not
//            //include that exception as inner exceptions are reported.
//            catch (OperationCanceledException e)
//            {
//                if(flag)
//                    throw new InvalidOperationException("sss", e);//OK
//
//                if(flag2)
//                    throw new InvalidOperationException("sss");//BAD
//
//                throw new InvalidOperationException();//BAD
//            }
//        }

        //public void Test15()
        //{
        //    ThrowingMethodWithNoComment();
        //}

        //public void Test16()
        //{
        //    ThrowingMethodWithComment();
        //}

        //public void Test17()
        //{
        //    try
        //    {
        //        ThrowingMethodWithNoComment();
        //    }
        //    catch(InvalidOperationException e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

        //public void Test18()
        //{
        //    try
        //    {
        //        ThrowingMethodWithComment();
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

        //public void Test19()
        //{
        //    try
        //    {
        //        throw new InvalidOperationException();
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
    }
}
