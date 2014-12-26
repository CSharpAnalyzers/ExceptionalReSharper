using System;

namespace Exceptional.Playground
{
    class EventMayThrowException
    {
        public void Foo()
        {
            var copy = MyEvent;
            if (copy != null)
                copy(); // Suggestion: May throw System.Exception
        }
        
        public event Action MyEvent;
    }
}
