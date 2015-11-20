using System;
using System.Security;

namespace Exceptional.Playground
{
    public class InnerExceptions
    {
        /// <exception cref="SecurityException">Sample. </exception>
        public void Test1()
        {
            try
            {
            }
            catch (ArgumentException abc)
            {
                throw new SecurityException(); // Warning: Inner exception not set => fix should insert message and inner exception
            }
        }

        /// <exception cref="SecurityException">Sample. </exception>
        public void Test2()
        {
            try
            {
            }
            catch (ArgumentException abc)
            {
                throw new SecurityException("Abc"); // Warning: Inner exception not set => fix should insert only inner exception
            }
        }

        /// <exception cref="SecurityException">Sample. </exception>
        public void Test3()
        {
            try
            {
            }
            catch (ArgumentException exception)
            {
                throw new SecurityException("Test", exception); // No warning: Inner exception is set
            }
        }

        /// <exception cref="SecurityException">Sample. </exception>
        public void Test4()
        {
            try
            {
            }
            catch
            {
                throw new SecurityException("Test"); // Warning: Inner exception not set
            }
        }
    }
}
