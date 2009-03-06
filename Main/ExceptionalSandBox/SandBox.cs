using System;

namespace ExceptionalSandBox
{
    public class SandBox
    {
        /// <summary></summary>
        /// <exception cref="InvalidOperationException">Test</exception>
        /// <exception cref="ArgumentException">Test</exception>
        /// <exception cref="ObjectDisposedException">Test</exception>
        public void Test13()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (OperationCanceledException)//BAD
            {
                Console.WriteLine("TRY");
                throw;//BAD
            }
        }



//        public void Test08()
//        {
//            try
//            {
//                Console.WriteLine("TRY");
//            }
//            //We are catching Exception type and this is not recommended.
//            catch (Exception)//BAD
//            {
//                Console.WriteLine("CATCH");
//            }
//        }

//        public void Test10()
//        {
//            try
//            {
//                Console.WriteLine("TRY");
//            }
//            catch (OperationCanceledException ex)//OK
//            {
//                try 
//                {
//                    //We are throwing InvalidOperationexception that is correctly documented and contains inner exception.
//                    throw new InvalidOperationException("Test", ex);//OK
//                }
//                //Swallowing exception is not recommended.
//                catch (InvalidOperationException) { }//BAD
//            }
//        }

//        public void Test11()
//        {
//            try
//            {
//                Console.WriteLine("TRY");
//            }
//            //Swallowing exception is not recommended.
//            catch//BAD
//            {
//                Console.WriteLine("TRY");
//            }
//        }
//
//        public void Test12()
//        {
//            try
//            {
//                Console.WriteLine("TRY");
//            }
//            catch(InvalidOperationException)//OK
//            {
//                Console.WriteLine("TRY");
//                //We are rethrowing, but the exception is not documented.
//                throw;//BAD
//            }
//        }

        

//        /// <summary></summary>
//        /// <exception cref="InvalidOperationException">Test</exception>
//        public void Test14()
//        {
//            try
//            {
//                Console.WriteLine("TRY");
//            }
//            catch (InvalidOperationException)//OK
//            {
//                Console.WriteLine("TRY");
//                //We are rethrowing, it's OK.
//                throw;//OK
//            }
//        }
//
//        public void Test15()
//        {
//            try
//            {
//                Console.WriteLine("TRY");
//            }
//            catch(InvalidOperationException)//BAD
//            {
//                
//            }
//            catch(Exception)//BAD
//            {
//                Console.WriteLine("TRY");
//            }
//        }
//
//        /// <exception cref="InvalidOperationException">Test</exception>//OK
//        public void Test16() 
//        {
//            try
//            {
//                throw new OperationCanceledException();//OK
//            }
//            //All ecxeptions thrown from inside of catch clause that do not 
//            //have a variable defined are reported as missing inner exception.
//            catch (OperationCanceledException)//BAD
//            {
//                throw new InvalidOperationException();//BAD
//            }
//        }
//
//        /// <exception cref="InvalidOperationException">Test</exception>//OK
//        public void Test17()
//        {
//            try
//            {
//                throw new OperationCanceledException();//OK
//            }
//            //All ecxeptions thrown from inside of catch clause that do not 
//            //have a variable defined are reported as missing inner exception.
//            catch (OperationCanceledException exmy)//BAD
//            {
//                throw new InvalidOperationException();//BAD
//            }
//        }
//
//        /// <exception cref="InvalidOperationException">Test</exception>//OK
//        public void Test18()
//        {
//            try
//            {
//                throw new OperationCanceledException();//OK
//            }
//            //All ecxeptions thrown from inside of catch clause that do not 
//            //have a variable defined are reported as missing inner exception.
//            catch//BAD
//            {
//                //FIX: should add (Exception e) to the catch an proper parameters to throw.
//                throw new InvalidOperationException();//BAD
//            }
//        }

//        /// <exception cref="Exception">Test</exception>//OK
//        public void Test17()
//        {
//            try
//            {
//                Console.WriteLine();
//            }
//            //Rethrow from general clause
//            catch//BAD
//            {
//                Console.WriteLine();
//                throw;//OK
//            }
//        }
//
//        /// <exception cref="InvalidOperationException">Test</exception>//OK
//        public void Test17(bool flag, bool flag2)
//        {
//            try
//            {
//                Console.WriteLine();
//            }
//            //All ecxeptions thrown from inside of catch clause that do not
//            //include that exception as inner exceptions are reported.
//            catch (OperationCanceledException e)//BAD
//            {
//                if(flag)
//                    throw new InvalidOperationException("Test", e);//OK
//
//                if(flag2)
//                    throw new InvalidOperationException("Test");//BAD
//
//                throw new InvalidOperationException();//BAD
//            }
//        }

        //public void Test22()
        //{
        //    ThrowingMethodWithNoComment();
        //}

        //public void Test23()
        //{
        //    ThrowingMethodWithComment();
        //}

        //public void Test24()
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

        //public void Test25()
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
    }
}
