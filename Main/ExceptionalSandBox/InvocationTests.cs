using System;

namespace ExceptionalSandBox
{
    public class InvocationTests
    {
        /// <exception cref="System.InvalidOperationException">Thrown when there was an accident.</exception>
        public void ReferenceMethod()
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="System.InvalidOperationException">Thrown when there was an accident.</exception>
        public string ReferenceProperty
        {
            get { throw new InvalidOperationException(); }
            set { throw new InvalidOperationException(); }
        }

        /// <exception cref="System.InvalidOperationException">Thrown when ...</exception>//OK
        public void Test01()
        {
            ReferenceMethod();//OK
        }

        public void Test02()
        {
            ReferenceMethod(); //BAD
        }

        public void Test03()
        {
            try
            {
                ReferenceMethod(); //OK
            }
            catch(Exception) {}
        }
    }
}