using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Exceptional.Playground.Fixed
{
    public class CSharp7Features : INotifyPropertyChanged
    {
        private string _property1;
        private string _property2;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Property1
        {
            get => this._property1;
            set => this._property1 = value ?? throw new NullReferenceException("Foo bar"); // Warning: Should insert with "set"
        }

        public string Property2
        {
            get => this._property2;

            set
            {
                this._property2 = value;
                this.OnPropertyChanged(); // Warning
            }
        }

        /// <exception cref="Exception" accessor="set">A delegate callback throws an exception.</exception>
        public string Property3
        {
            get => this._property2;
            set
            {
                this._property2 = value;
                this.OnPropertyChanged();
            }
        }


        public void Method1()
        {
            this.Property1 = this.Property3; // No warning
        }

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged2([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Warning
        }
    }
}
