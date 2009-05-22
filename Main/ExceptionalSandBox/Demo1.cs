using System;
using System.IO;

namespace ExceptionalSandBox
{
    class Demo1
    {
        public void Demo01()//OK
        {
            //We do not throw and there is no documentation of exceptions.    
        }

        public void Demo02()
        {
            //We throw but there is no documentation for the exception thrown.
            throw new InvalidOperationException("Message");
        }

        /// <summary>This method...</summary>
        /// <exception cref="InvalidOperationException">Thrown when...</exception> //BAD
        public void Demo03()
        {
            //We are not throwing so the exception documentation is invalid.
        }

        public void Demo04()
        {
            File.ReadAllLines("asd");
        }       
    }
}
