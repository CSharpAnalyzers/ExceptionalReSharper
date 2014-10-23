using System;
using System.Security;

namespace Exceptional.Playground.Issues
{
    // Low priority because exceptions on events are very rare

    class EventWithExceptionNotConsidered
    {
        public void Foo()
        {
            MyEvent(); // Issue: Should show warning => exception of event not documented
        }

        /// <summary>Occurs when foo. </summary>
        /// <exception cref="SecurityException">The condition. </exception>
        public event Action MyEvent;
    }
}
