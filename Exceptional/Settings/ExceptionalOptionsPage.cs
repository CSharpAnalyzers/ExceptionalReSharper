using System;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.UI.Application;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;
using JetBrains.UI.Options.OptionPages;
using ReSharper.Exceptional.Images;
using ReSharper.Exceptional.Settings.Views;

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