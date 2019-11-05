using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Exceptional.Playground.Fixed
{
    public class NameofOperatorShallBeIgnored : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <exception cref="T:System.InvalidOperationException" accessor="get">Condition.</exception>
        public string SomeProperty
        {
            // no warning shall be shown
            get => throw new InvalidOperationException("");
            // no warning shall be shown on SomeProperty, because nameof does not access the getter
            set => OnPropertyChanged(nameof(SomeProperty));
        }

        /// <exception cref="T:System.InvalidOperationException" accessor="get">Condition.</exception>
        public string SomeOtherProperty
        {
            // no warning shall be shown
            get => throw new InvalidOperationException("");
            set
            {
                // warning shall be shown on SomeOtherProperty that InvalidOperationException is not documented
                if (SomeOtherProperty == value)
                {
                    return;
                }
            }
        }

        /// <exception cref="T:System.InvalidOperationException" accessor="get">Condition.</exception>
        /// <exception cref="T:System.InvalidOperationException" accessor="set">Condition.</exception>
        public string YetAnotherProperty
        {
            // no warning shall be shown
            get => throw new InvalidOperationException("");
            set
            {
                // no warning shall be shown
                if (YetAnotherProperty == value)
                {
                    return;
                }
            }
        }

        /// <exception cref="T:System.Exception">A delegate callback throws an exception.</exception>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Test(int i, string a)
        {
        }
    }
}