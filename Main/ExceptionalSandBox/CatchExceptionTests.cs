using System;
using System.IO;
using System.Security;

namespace ExceptionalSandBox
{
    public class CatchExceptionTests
    {
        public void Test01()
        {
            try
            {
                File.ReadAllLines("asd");//TODO: DOES NOT WORK
            }
            catch (InvalidOperationException) {}
        }

        //public void Test02()
        //{
        //    File.ReadAllLines("asd");//TODO: DOES NOT WORK
        //}

        //public void Test03()
        //{
        //    try
        //    {
        //        try
        //        {
        //            File.ReadAllLines("asd");//TODO: DOES NOT WORK
        //        }
        //        catch (InvalidOperationException exception)
        //        {
        //        }
        //    }
        //    catch (IOException exception)
        //    {
        //    }
        //}

        //public void Test04()
        //{
        //    try
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    catch (InvalidOperationException) { }
        //}

        //public void Test05()
        //{
        //    throw new ArgumentNullException();
        //}


        //public void Test06()
        //{
        //    try
        //    {
        //        try
        //        {
        //            throw new ArgumentNullException();
        //        }
        //        catch (InvalidOperationException exception)
        //        {
        //        }
        //    }
        //    catch (IOException exception)
        //    {
        //    }
        //}
    }
}
