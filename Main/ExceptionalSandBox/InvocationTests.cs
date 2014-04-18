using System;
using System.IO;
using System.Xml;

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
            catch {}
        }

        public void Test04()
        {
            try
            {
                ReferenceMethod(); //OK
            }
            catch(Exception) { }//BAD
        }

        public void Test05()
        {
            try
            {
                ReferenceMethod(); //OK
            }
            catch (InvalidOperationException) { }
        }

        public void Test06()
        {
            try
            {
                ReferenceMethod(); //BAD
            }
            catch (ArgumentNullException) { }
        }

        public void Test07()
        {
            Decimal.Parse("3.4");//TODO: DOES NOT WORK
        }

        public void Test08()
        {
            this.ReferenceProperty = "sd";
        }

        public void Test09()
        {
            var s = this.ReferenceProperty;
        }
    }
}