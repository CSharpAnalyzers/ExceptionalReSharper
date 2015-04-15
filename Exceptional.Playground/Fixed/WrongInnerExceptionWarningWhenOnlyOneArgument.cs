using System;

namespace Exceptional.Playground.Fixed
{
    class WrongInnerExceptionWarningWhenOnlyOneArgument
    {
        // https://exceptional.codeplex.com/workitem/11012

        /// <exception cref="OuterException">Test. </exception>
        public void Foo()
        {
            try
            {

            }
            catch (Exception e)
            {
                throw new OuterException(e); // No warning
            }
        }
    }

    public class OuterException : Exception
    {
        public OuterException(Exception innerException) : base("Test", innerException)
        {
            
        }
    }
}
