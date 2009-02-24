using System;

namespace ExceptionalSandBox
{
    public class SandBox
    {
//        public void Test01()//OK
//        {
//            //We do not throw and there is no documentation of exceptions.    
//        }
//
//        public void Test02()
//        {
//            //We throw but there is no documentation for the exception thrown.
//            throw new InvalidOperationException("Message");//BAD
//        }
//
//        /// <summary>This method...</summary>
//        public void Test03()
//        {
//            //We throw and there is some documentation but still the exception thrown is not documented.
//            throw new InvalidOperationException("Message");//BAD
//        }
//
//        /// <summary>This method...</summary>
//        /// <exception cref="InvalidOperationException">Thrown when....</exception>//OK
//        public void Test04()
//        {
//            //We are good. Thrown exception is documented.
//            throw new InvalidOperationException("Message");//OK
//        }
//
//        /// <summary>This method...</summary>
//        /// <exception cref="InvalidOperationException">Thrown when....</exception>//OK
//        public void Test05()
//        {
//            //We are good. Thrown exception is documented.
//            var ex = new InvalidOperationException("Message");
//            throw ex;//OK
//        }
//
//        /// <summary>This method...</summary>
//        /// <exception cref="InvalidOperationException">Thrown when....</exception>//BAD
//        public void Test06()
//        {
//            //Despite of the fact that we are creating an InvalidOperationException we are throwing Exception.
//            //So in fact we are throwing Exception and the documentation contains entry that is not valid.
//            Exception ex = new InvalidOperationException();
//            throw ex;//BAD
//        }
//
//        /// <summary>This method...</summary>
//        /// <exception cref="InvalidOperationException">Thrown when...</exception> //BAD
//        public void Test07()
//        {
//            //We are not throwing so the exception documentation is invalid.
//        }
//
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
//
//        /// <exception cref="InvalidOperationException">Test</exception>//OK
//        public void Test09()
//        {
//            try
//            {
//                Console.WriteLine("TRY");
//            }
//            //We are catching Exception type and this is not recommended.
//            catch (Exception ex)//BAD
//            {
//                //We are throwing InvalidOperationexception that is correctly documented and contains inner exception.
//                throw new InvalidOperationException("Test", ex);//OK
//            }
//        }
//
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
//
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

        public void Test12()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch(InvalidOperationException)//OK
            {
                Console.WriteLine("TRY");
                //We are rethrowing, but the exception is not documented.
                throw;//BAD
            }
        }

        public void Test13()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (InvalidOperationException ex)//OK
            {
                //We are throwing but the exception is not documented.
                throw new InvalidOperationException("Message", ex);//BAD
            }
        }

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
//                //We should include a catched exception as inner exception
//                throw new InvalidOperationException("sss");//BAD
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
