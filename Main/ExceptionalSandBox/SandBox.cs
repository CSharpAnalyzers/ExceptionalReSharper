using System;

namespace ExceptionalSandBox
{
    public class SandBox
    {
        //public void NotThrowingMethod()
        //{

        //}

        //public void ThrowingMethodWithNoComment()
        //{
        //    throw new InvalidOperationException();
        //}

        ///// <summary>This method...</summary>
        //public void ThrowingMethodWithPartialComment()
        //{
        //    throw new InvalidOperationException();
        //}

        ///// <summary>This method...</summary>
        ///// <exception cref="InvalidOperationException">Thrown when....</exception>
        //public void ThrowingMethodWithComment()
        //{
        //    throw new InvalidOperationException();
        //}

        ///// <summary>This method...</summary>
        ///// <exception cref="InvalidOperationException">Thrown when...</exception>
        //public void NotThrowingMethodWithComment()
        //{
        //    //look I'm not throwing
        //}

        //public void MethodWIthTryCatch()
        //{
        //    try
        //    {
        //        Console.WriteLine("TRY");
        //    }
        //    catch (Exception)
        //    {
        //        Console.WriteLine("CATCH");
        //    }
        //}

        //public void MethodThrowingFromCatch()
        //{
        //    try
        //    {
        //        Console.WriteLine("TRY");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException("sss", ex);
        //    }
        //}

        //public void MethodThrowingFromCatchWithTry()
        //{
        //    try
        //    {
        //        Console.WriteLine("TRY");
        //    }
        //    catch (OperationCanceledException ex)
        //    {
        //        try
        //        {
        //            throw new InvalidOperationException("sss", ex);
        //        }
        //        catch(InvalidOperationException) {}
        //    }
        //}

        //public void MethodWithCatchAll()
        //{
        //    try
        //    {
        //        Console.WriteLine("TRY");
        //    }
        //    catch
        //    {
        //        Console.WriteLine("TRYs");
        //    }
        //}

        //public void MethodWithCatchAllExplicit()
        //{
        //    try
        //    {
        //        Console.WriteLine("TRY");
        //    }
        //    catch(InvalidOperationException)
        //    {
                
        //    }
        //    catch(Exception)
        //    {
        //        Console.WriteLine("TRYs");
        //    }
        //}

        /// <summary></summary>
        /// <exception cref="InvalidOperationException">...</exception>
        public void MethodThrowingFromCatchHidingSourceException()
        {
            try
            {
                throw new OperationCanceledException();
            }
            catch (OperationCanceledException)
            {
                //We should include a catched exception as inner exception
                throw new InvalidOperationException("sss");
            }
        }

        //public void MethodUsingMethodThrowingExceptionWithNoComment()
        //{
        //    ThrowingMethodWithNoComment();
        //}

        //public void MethodUsingMethodThrowingExceptionWithComment()
        //{
        //    ThrowingMethodWithComment();
        //}

        //public void MethodUsingCatchingMethodThrowingExceptionWithNoComment()
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

        //public void MethodUsingCatchingMethodThrowingExceptionWithComment()
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

        //public void MethodThrowingFromTryWithProperCatch()
        //{
        //    try
        //    {
        //        throw new InvalidOperationException();
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
    }
}
