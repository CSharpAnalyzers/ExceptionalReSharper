using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;

namespace ReSharper.Exceptional.Options
{
    [OptionsPage(Pid, Name, null, Sequence = 5.0)]
    public class ExceptionalOptionsPage : AEmptyOptionsPage
    {
        public const string Pid = "Exceptional";
        public const string Name = "Exceptional";
    }
}