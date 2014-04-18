using System;

namespace ExceptionalSandBox
{
    public class Demo2
    {
        /// <exception cref="InvalidOperationException">Description</exception>//OK
        public void Demo01()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (OperationCanceledException oce)//OK
            {
                throw new InvalidOperationException("Message", oce);//OK
            }
        }

        /// <exception cref="System.InvalidOperationException">Thrown when test</exception>//OK
        public void Demo02()
        {
            try
            {
                Console.WriteLine("TRY");
            }
            catch (OperationCanceledException exception)//OK
            {
                throw new InvalidOperationException("Message", exception);//BAD
            }
        }

        public void Demo03()
        {
            try
            {
                var name = "Bartek";
            }
            catch (OperationCanceledException exception)//OK
            {
                throw new InvalidOperationException("See inner exception for details.", exception);//BAD
            }
        }

        public void Demo04()
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