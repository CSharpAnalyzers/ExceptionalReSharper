using System;
using JetBrains.Application.UI.Components.UIApplication;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;
using JetBrains.DataFlow;

using ReSharper.Exceptional.Settings.Views;
using yWorks.Support.Annotations;

namespace ReSharper.Exceptional.Settings
{
    [OptionsPage(Pid, "Exceptional", typeof(UnnamedThemedIcons.ExceptionalSettings), ParentId = EnvironmentPage.Pid, Sequence = 100)]
    public class ExceptionalOptionsPage : AOptionsPage
    {
        private const string Pid = "ExceptionalSettings";

        public ExceptionalOptionsPage([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settings, UIApplication environment)
            : base(lifetime, environment, Pid)
        {
            if (lifetime == null)
                throw new ArgumentNullException("lifetime");

            Control = new SettingsView(lifetime, settings);
        }
    }
}