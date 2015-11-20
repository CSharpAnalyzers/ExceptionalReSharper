using System;
using System.IO;
using System.Security;

namespace Exceptional.Playground
{
    public class UnnecessaryExceptionDocumented
    {
        // Three warnings: Exception is not thrown but documented

        /// <summary>Abc. </summary>
        /// <exception cref="Exception">Sample. </exception>
        /// <exception cref="SecurityException" />
        /// <exception cref="IOException">Sample. </exception>
        public void Test2()
        {
        }
    }
}
