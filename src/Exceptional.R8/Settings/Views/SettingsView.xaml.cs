using System;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.Controls;
using JetBrains.UI.Options;

namespace ReSharper.Exceptional.Settings.Views
{
    /// <summary>Interaction logic for SettingsView.xaml</summary>
    public partial class SettingsView : UserControl
    {
        private readonly OptionsSettingsSmartContext _settings;
        private readonly Lifetime _lifetime;

        public SettingsView(Lifetime lifetime, OptionsSettingsSmartContext settings)
        {
            InitializeComponent();

            _lifetime = lifetime;
            _settings = settings;

            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.DelegateInvocationsMayThrowExceptions,
                DelegateInvocationsMayThrowExceptions, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);
            
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements,
                IsDocumentationOfExceptionSubtypeSufficientForThrowStatements, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions,
                IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.InspectPublicMethods,
                InspectPublicMethods, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.InspectInternalMethods,
                InspectInternalMethods, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.InspectProtectedMethods,
                InspectProtectedMethods, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.InspectPrivateMethods,
                InspectPrivateMethods, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.OptionalExceptions2,
                OptionalExceptions, TextBox.TextProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.UseDefaultOptionalExceptions2,
                UseOptionalExceptionsDefaults, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.OptionalMethodExceptions2,
                OptionalMethodExceptions, TextBox.TextProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.UseDefaultOptionalMethodExceptions2,
                UseOptionalMethodExceptionsDefaults, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.AccessorOverrides2,
                AccessorOverrides, TextBox.TextProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.UseDefaultAccessorOverrides2,
                UseDefaultAccessorOverrides, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

            //settings.Changed.Advise(lifetime, delegate { Dispatcher.BeginInvoke((Action)(UpdateTextFields)); });
            lifetime.AddAction(delegate
            {
                // TODO: Force rescan of all documents if settings have changed
            });
        }

        private void ShowPredefinedOptionalExceptions(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ExceptionalSettings.DefaultOptionalExceptions);
        }

        private void ShowPredefinedOptionalMethodAndPropertyExceptions(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ExceptionalSettings.DefaultOptionalMethodExceptions);
        }

        private void ShowPredefinedAccessorOverrides(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ExceptionalSettings.DefaultAccessorOverrides);
        }
    }
}
