using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.Base2;
using JetBrains.TextControl;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
{
    internal abstract class SingleActionFix : OneItemBulbActionImpl, IBulbItem, IQuickFix
    {
        protected override IBulbItem GetBulbItem(JetBrains.Util.IUserDataHolder cache)
        {
            return this;
        }

        public abstract void Execute(ISolution solution, ITextControl textControl);
        public abstract string Text { get; }
    }
}