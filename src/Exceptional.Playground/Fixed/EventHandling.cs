using System;

namespace Exceptional.Playground.Fixed
{
    class EventHandling
    {
        public void Foo(Action action)
        {
            var copy = Hello; // No warning
            if (copy != null)
                copy(); // Suggestion
        }

        public void Bar(Action action)
        {
            action(); // Suggestion: Calling delegate may throw exception
        }

        public event Action Hello;
    }
}
