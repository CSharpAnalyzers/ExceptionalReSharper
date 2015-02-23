using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class IgnoreWarningShouldWork
    {
        public void Foo()
        {
            // ReSharper disable once ExceptionNotDocumented
            throw new SecurityException("Test"); // no warning
        }
        
        public void Bar()
        {
            throw new SecurityException("Test"); // warning
        }
    }
}