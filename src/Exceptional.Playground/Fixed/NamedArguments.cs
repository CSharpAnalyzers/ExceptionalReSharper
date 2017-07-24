namespace Exceptional.Playground.Fixed
{
    using System;

    public class NamedArguments
    {
        /// <exception cref="InvalidOperationException">Condition.</exception>
        public void Method1()
        {
            try
            {
                var d = decimal.Parse("123");
            }
            catch (ArgumentNullException ane)
            {
                // no warning, inner exception is the caught exception
                throw new InvalidOperationException(ane.Message, ane);
            }
            catch (FormatException fe)
            {
                // no warning, inner exception is the caught exception
                throw new InvalidOperationException(message: fe.Message, innerException: fe); 
            }
            catch (OverflowException oe)
            {
                // no warning, inner exception is the caught exception
                throw new InvalidOperationException(innerException:oe, message:oe.Message);
            }
        }

        /// <exception cref="InvalidOperationException">Condition.</exception>
        public void Method2()
        {
            try
            {
                var d = decimal.Parse("123");
            }
            catch (ArgumentNullException ane)
            {
                // warning, Caught Exception should be passed as inner exception
                throw new InvalidOperationException(ane.Message, new Exception("123"));
            }
            catch (FormatException fe)
            {
                var newfe = new FormatException("123");

                // warning, Caught Exception should be passed as inner exception
                throw new InvalidOperationException(message: fe.Message, innerException: newfe);
            }
            catch (OverflowException oe)
            {
                // warning, Caught Exception should be passed as inner exception
                throw new InvalidOperationException(innerException: new Exception("123"), message: oe.Message);
            }
        }
    }
}
