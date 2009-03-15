using System;

namespace ExceptionalSandBox
{
    public class CommentsTests
    {
        public void Test01()//OK
        {
            //We do not throw and there is no documentation of exceptions.    
        }
        
        public void Test02()
        {
            //We throw but there is no documentation for the exception thrown.
            throw new InvalidOperationException("Message");//BAD
        }

        /// <summary>This method...</summary>
        public void Test03()
        {
            //We throw and there is some documentation but still the exception thrown is not documented.
            throw new InvalidOperationException("Message");//BAD
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when....</exception>//OK
        public void Test04()
        {
            //We are good. Thrown exception is documented.
            throw new InvalidOperationException("Message");//OK
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when....</exception>//OK
        public void Test05()
        {
            //We are good. Thrown exception is documented.
            var ex = new InvalidOperationException("Message");
            throw ex;//OK
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when....</exception>//BAD
        public void Test06()
        {
            //Despite of the fact that we are creating an InvalidOperationException we are throwing Exception.
            //So in fact we are throwing Exception and the documentation contains entry that is not valid.
            Exception ex = new InvalidOperationException();
            throw ex;//BAD
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when...</exception> //BAD
        public void Test07()
        {
            //We are not throwing so the exception documentation is invalid.
        }

        /// <exception cref="InvalidOperationException">Test</exception>//OK
        public void Test08()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            //We are catching Exception type and this is not recommended.
            catch (Exception ex)//BAD
            {
                //We are throwing InvalidOperationexception that is correctly documented and contains inner exception.
                throw new InvalidOperationException("Test", ex);//OK
            }
        }
    }
}