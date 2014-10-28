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

            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.EventInvocationsMayThrowExceptions,
                EventInvocationsMayThrowExceptions, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);
            
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

            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.OptionalExceptions,
                OptionalExceptions, TextBox.TextProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.UseDefaultOptionalExceptions,
                UseOptionalExceptionsDefaults, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.OptionalMethodExceptions,
                OptionalMethodExceptions, TextBox.TextProperty);
            settings.SetBinding(lifetime, (ExceptionalSettings x) => x.UseDefaultOptionalMethodExceptions,
                UseOptionalMethodExceptionsDefaults, CheckBoxDisabledNoCheck2.IsCheckedLogicallyDependencyProperty);

            settings.Changed.Advise(lifetime, delegate { Dispatcher.BeginInvoke((Action)(UpdateTextFields)); });
            lifetime.AddAction(delegate
            {
                // TODO: Force rescan of all documents if settings have changed
            });

            UpdateTextFields();
        }

        private void UpdateTextFields()
        {
            var settings = _settings.GetKey<ExceptionalSettings>(SettingsOptimization.OptimizeDefault);
            OptionalExceptions.IsEnabled = !settings.UseDefaultOptionalExceptions;
            OptionalMethodExceptions.IsEnabled = !settings.UseDefaultOptionalMethodExceptions;
        }

        private void OnResetOptionalExceptions(object sender, RoutedEventArgs e)
        {
            _settings.ResetValue((ExceptionalSettings x) => x.OptionalExceptions);
            _settings.SetBinding(_lifetime, (ExceptionalSettings x) => x.OptionalExceptions,
                OptionalExceptions, TextBox.TextProperty);
        }

        private void OnResetOptionalMethodExceptions(object sender, RoutedEventArgs e)
        {
            _settings.ResetValue((ExceptionalSettings x) => x.OptionalMethodExceptions);
            _settings.SetBinding(_lifetime, (ExceptionalSettings x) => x.OptionalMethodExceptions,
                OptionalMethodExceptions, TextBox.TextProperty);
        }
    }
}
