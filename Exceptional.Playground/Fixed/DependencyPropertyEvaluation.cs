using System;
using System.Windows;

namespace Exceptional.Playground.Fixed
{
    public class DependencyPropertyEvaluation : DependencyObject
    {
        public static readonly DependencyProperty SampleProperty =
            DependencyProperty.Register("Sample", typeof(Boolean), typeof(DependencyPropertyEvaluation));

        public Boolean Sample
        {
            get { return (Boolean)GetValue(SampleProperty); } // Hint
            set { SetValue(SampleProperty, value); } // Hint
        }
    }
}
