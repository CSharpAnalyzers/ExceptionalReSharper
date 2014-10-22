using System;

namespace Exceptional.Playground
{
    class CopyExceptionMessageToDocumentation
    {
        public void Foo()
        {
            throw new ArgumentException("abc"); // When inserting exception to documentation then "abc" should be used as exception description/condition. 
        }
    }
}
