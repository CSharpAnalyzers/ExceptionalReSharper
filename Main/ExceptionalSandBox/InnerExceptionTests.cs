using System;

namespace ExceptionalSandBox
{
    public class InnerExceptionTests
    {
        /// <exception cref="System.InvalidOperationException">Thrown when test</exception>//OK
        public void Test01()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch(OperationCanceledException oce)//OK
            {
                throw new InvalidOperationException("Message", oce);//OK
            }
        }

        /// <exception cref="System.InvalidOperationException">Thrown when test</exception>//OK
        public void Test02()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (OperationCanceledException)//OK
            {
                throw new InvalidOperationException("Message");//BAD
            }
        }

        /// <exception cref="System.InvalidOperationException">Thrown when test</exception>//OK
        public void Test03()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (OperationCanceledException)//OK
            {
                throw new InvalidOperationException();//BAD
            }
        }

        /// <exception cref="System.InvalidOperationException">Thrown when test</exception>//OK
        public void Test04()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch//BAD
            {
                throw new InvalidOperationException();//BAD
            }
        }
    }
}