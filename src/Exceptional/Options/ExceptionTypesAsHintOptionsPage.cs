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
    [OptionsPage(Pid, Name, typeof(UnnamedThemedIcons.ExceptionalSettings), ParentId = ExceptionalOptionsPage.Pid, Sequence = 2.0)]
    public class ExceptionTypesAsHintOptionsPage : BeSimpleOptionsPage
    {
        public const string Pid = "Exceptional::ExceptionTypesAsHint";
        public const string Name = "Optional Exceptions (Global)";

        public ExceptionTypesAsHintOptionsPage(Lifetime lifetime, OptionsPageContext optionsPageContext, OptionsSettingsSmartContext optionsSettingsSmartContext) : base(lifetime, optionsPageContext, optionsSettingsSmartContext, true)
        {
            AddText(OptionsLabels.ExceptionTypesAsHint.Description);

            CreateCheckboxUsePredefined(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);

            AddButton(OptionsLabels.ExceptionTypesAsHint.ShowPredefined, ShowPredefined);

            AddSpacer();

            AddText(OptionsLabels.ExceptionTypesAsHint.Note);
            CreateRichTextExceptionTypesAsHint(lifetime, optionsSettingsSmartContext.StoreOptionsTransactionContext);
        }

        private void ShowPredefined()
        {
            string content = ReSharper.Exceptional.Settings.ExceptionalSettings.DefaultOptionalExceptions;

            MessageBox.ShowInfo(content);
        }

        private void CreateCheckboxUsePredefined(Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<bool> property = new Property<bool>(lifetime, "Exceptional::ExceptionTypesAsHint::UsePredefined");
            property.SetValue(storeOptionsTransactionContext.GetValue((Settings.ExceptionalSettings key) => key.UseDefaultOptionalExceptions2));

            property.Change.Advise(lifetime, a =>
            {
                if (!a.HasNew) return;
                
                storeOptionsTransactionContext.SetValue((Settings.ExceptionalSettings key) => key.UseDefaultOptionalExceptions2, a.New);
            });

            AddBoolOption((Settings.ExceptionalSettings key) => key.UseDefaultOptionalExceptions2, OptionsLabels.ExceptionTypesAsHint.UsePredefined);
        }

        private void CreateRichTextExceptionTypesAsHint(Lifetime lifetime, IContextBoundSettingsStoreLive storeOptionsTransactionContext)
        {
            IProperty<string> property = new Property<string>(lifetime, "Exceptional::ExceptionTypesAsHint::ExceptionTypes");
            property.SetValue(storeOptionsTransactionContext.GetValue((Settings.ExceptionalSettings key) => key.OptionalExceptions2));

            property.Change.Advise(lifetime, a =>
            {
                if (!a.HasNew) return;

                storeOptionsTransactionContext.SetValue((Settings.ExceptionalSettings key) => key.OptionalExceptions2, a.New);
            });

            var textControl = BeControls.GetTextControl(isReadonly:false);

            textControl.Text.SetValue(property.GetValue());
            textControl.Text.Change.Advise(lifetime, str =>
            {
                storeOptionsTransactionContext.SetValue((Settings.ExceptionalSettings key) => key.OptionalExceptions2, str);
            });

            AddControl(textControl);
        }
    }
}