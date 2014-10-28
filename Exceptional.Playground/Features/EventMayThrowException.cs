using System;

namespace Exceptional.Playground.Features
{
    class EventMayThrowException
    {
        public void Foo()
        {
            MyEvent(); // Suggestion: May throw System.Exception
        }
        
        public event Action MyEvent;
    }
}
