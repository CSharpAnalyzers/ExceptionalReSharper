using System;

namespace ExceptionalSandBox
{
    public class PropertiesCommentsTests
    {
        public string Property01 { get; set; }

        /// <exception cref="InvalidOperationException">Test</exception>//TODO: DOES NOT WORK
        public string Property02 { get; set; }

        public string Property03
        {
            get
            {
                throw new InvalidOperationException();//BAD
            }
        }

        public string Property04
        {
            set
            {
                throw new InvalidOperationException();//BAD
            }
        }

        public string Property05
        {
            get
            {
                throw new ArgumentNullException();//BAD
            }
            set
            {
                throw new InvalidOperationException();//BAD
            }
        }

        /// <exception cref="ArgumentNullException">Test</exception>//OK
        public string Property06
        {
            get
            {
                throw new ArgumentNullException();//OK
            }
            set
            {
                throw new InvalidOperationException();//BAD
            }
        }

        /// <exception cref="ArgumentNullException">Test</exception>//OK
        /// <exception cref="InvalidOperationException">Test</exception>//OK
        public string Property07
        {
            get
            {
                throw new ArgumentNullException();//OK
            }
            set
            {
                throw new InvalidOperationException();//OK
            }
        }
    }
}