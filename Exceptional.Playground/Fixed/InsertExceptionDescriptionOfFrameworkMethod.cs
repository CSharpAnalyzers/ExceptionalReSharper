using System;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    class InsertExceptionDescriptionOfFrameworkMethod
    {
        public void Test01()
        {
            var x = AppDomain.CurrentDomain.FriendlyName; // Warning: Should insert "The operation is attempted on an unloaded application domain. " as exception description

            var y = AppDomain.CurrentDomain.FriendlyName.Split('a'); // Warning: On FriendlyName
            var z = AppDomain.CurrentDomain.FriendlyName.Length; // Warning: On FriendlyName

            var t = AppDomain.CurrentDomain.FriendlyName.Substring(6, -4); // Warning and hint: On FriendlyName and Substring()
            var k = Test02().Substring(6, -4); // Warning and hint: On Test02 and Substring()
            var u = Test03().FriendlyName; // Two warnings: On Test02 and FriendlyName
            var m = Test03().FriendlyName.Substring(6, -4); // Two warnings and one hint: On Test02, FriendlyName and Substring
        }

        /// <exception cref="SecurityException">Condition. </exception>
        public string Test02()
        {
            throw new SecurityException();
        }

        /// <exception cref="SecurityException">Condition. </exception>
        public AppDomain Test03()
        {
            throw new SecurityException();
        }
    }
}
