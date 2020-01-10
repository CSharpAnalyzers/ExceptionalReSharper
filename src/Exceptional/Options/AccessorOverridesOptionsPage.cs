using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.Rd.Base;
using JetBrains.Util;

namespace ReSharper.Exceptional.Options
{
    [OptionsPage(Pid, Name, typeof(UnnamedThemedIcons.ExceptionalSettings), ParentId = ExceptionalOptionsPage.Pid, Sequence = 4.0)]
    public class AccessorOverridesOptionsPage : BeSimpleOptionsPage
    {
        public const string Pid = "Exceptional::AccessorOverrides";
        public const string Name = "Accessor Overrides";

        public AccessorOverridesOptionsPage(Lifetime lifetime, OptionsPageContext optionsPageContext, OptionsSettingsSmartContext optionsSettingsSmartContext, bool wrapInScrollablePanel = true) : base(lifetime, optionsPageContext, optionsSettingsSmartContext, wrapInScrollablePanel)
        {
            AddText(OptionsLabels.AccessorOverrides.Description);

            CreateCheckboxUsePredefined(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);

            AddButton(OptionsLabels.AccessorOverrides.ShowPredefined, ShowPredefined);

            AddSpacer();

            AddText(OptionsLabels.AccessorOverrides.Note);
            CreateRichTextAccessorOverrides(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
        }

        private void ShowPredefined()
        {
            string content = ReSharper.Exceptional.Settings.ExceptionalSettings.DefaultAccessorOverrides;

            MessageBox.ShowInfo(content);
        }

        private void CreateCheckboxUsePredefined(in Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property = new Property<bool>(lifetime, "Exceptional::AccessorOverrides::UsePredefined");
            property.SetValue(storeOptionsTransactionContext.GetValue((Settings.ExceptionalSettings key) => key.UseDefaultAccessorOverrides2));

            property.Change.Advise(lifetime, a =>
            {
                if (!a.HasNew) return;

                storeOptionsTransactionContext.SetValue((Settings.ExceptionalSettings key) => key.UseDefaultAccessorOverrides2, a.New);
            });

            AddBoolOption((Settings.ExceptionalSettings key) => key.UseDefaultAccessorOverrides2, OptionsLabels.AccessorOverrides.UsePredefined);
        }

        private void CreateRichTextAccessorOverrides(in Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<string> property = new Property<string>(lifetime, "Exceptional::AccessorOverrides::AccessorOverrides");
            property.SetValue(storeOptionsTransactionContext.GetValue((Settings.ExceptionalSettings key) => key.AccessorOverrides2));

            property.Change.Advise(lifetime, a =>
            {
                if (!a.HasNew) return;

                storeOptionsTransactionContext.SetValue((Settings.ExceptionalSettings key) => key.AccessorOverrides2, a.New);
            });

            var textControl = BeControls.GetTextControl(isReadonly:false);

            textControl.Text.SetValue(property.GetValue());
            textControl.Text.Change.Advise(lifetime, str =>
            {
                storeOptionsTransactionContext.SetValue((Settings.ExceptionalSettings key) => key.AccessorOverrides2, str);
            });

            AddControl(textControl);
        }

    }
}