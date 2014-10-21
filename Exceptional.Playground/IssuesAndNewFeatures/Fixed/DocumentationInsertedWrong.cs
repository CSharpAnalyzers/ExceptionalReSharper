using System;

namespace Exceptional.Playground.IssuesAndNewFeatures.Fixed
{
    class DocumentationInsertedWrong
    {
        private string _property;

        /// <summary>a. </summary>
        private string Property
        {
            get
            {
                if (string.IsNullOrEmpty(_property))
                    throw new NullReferenceException("abc"); // Add exception documentation: Inserted on Foo(a, b) instead of Property
                return _property;
            }
            set { _property = value; }
        }

        /// <summary>b. </summary>
        /// <param name="a">a.</param>
        public DocumentationInsertedWrong(string a)
        {

        }

        /// <summary>c. </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        public DocumentationInsertedWrong(string a, string b)
        {

        }
    }
}
