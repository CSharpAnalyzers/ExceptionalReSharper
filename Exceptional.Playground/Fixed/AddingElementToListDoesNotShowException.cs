using System;
using System.Collections.Generic;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    class AddingElementToListDoesNotShowException
    {
        public void Foo()
        {
            var x = new Bar();
            var y = new List<object> { x.Foo, x.MyProperty, x.Test() }; // Two warnings: Exception of Test not documented
        }

        public class Bar
        {
            /// <exception cref="SecurityException">Condition. </exception>
            public string MyProperty
            {
                get
                {
                    throw new SecurityException();
                }
            }

            /// <exception cref="SecurityException">Condition. </exception>
            public string Test()
            {
                throw new SecurityException();
            }

            public Action Foo { get; set; }
        }
    }
}
