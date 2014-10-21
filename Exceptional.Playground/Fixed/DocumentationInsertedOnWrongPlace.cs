using System;

namespace Exceptional.Playground.Fixed
{
    public class DocumentationInsertedOnWrongPlace
    {
        private double[] _arrayField;

        public double[] ArrayField
        {
            get { return _arrayField; }
            set
            {
                if (value == null || value.Length != 2)
                    throw new ArgumentException("Bli bla blu"); // Bug: Inserting documentation inserts on Property2 and warning is shown 3 times
                _arrayField = value;
            }
        }

        public object Property1 { get; set; }

        /// <summary>Text</summary>
        public bool Property2 { get; set; }
    }
}
