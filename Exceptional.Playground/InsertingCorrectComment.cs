using System.Security;

namespace Exceptional.Playground
{
    public class InsertingCorrectComment
    {
        public void Foo()
        {
            Bar(); // Warning: When inserting exception documentation from this method, then the text below should be copied. 
        }

        /// <exception cref="SecurityException">This text should be inserted. </exception>
        public void Bar()
        {
            throw new SecurityException();
        }
    }
}
