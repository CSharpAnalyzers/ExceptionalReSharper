using System;

namespace Exceptional.Playground.Fixed
{
    class InsertExceptionDescriptionOfFrameworkMethod
    {
        public void Test01()
        {
            var x = AppDomain.CurrentDomain.FriendlyName; // Should insert "The operation is attempted on an unloaded application domain. " as exception description

            // see http://msdn.microsoft.com/en-us/library/vstudio/system.appdomain.friendlyname(v=vs.90).aspx
        }
    }
}
