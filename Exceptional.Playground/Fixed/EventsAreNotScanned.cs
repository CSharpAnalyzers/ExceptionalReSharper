using System;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    public class EventsAreNotScanned
    {
        public void Test()
        {
            throw new SecurityException(); // When inserting documentation then it is inserted on event...
        }

        /// <summary>Foo bar. </summary>
        public event Action<EventArgs> MyEvent;
    }
}
