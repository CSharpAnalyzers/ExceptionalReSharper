/// <copyright file="SingleActionFix.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.Base2;
using JetBrains.TextControl;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
{
    /// <summary>Base class for all fixes that serves only one <see cref="IBulbItem"/>.</summary>
    internal abstract class SingleActionFix : OneItemBulbActionImpl, IBulbItem, IQuickFix
    {
        protected override IBulbItem GetBulbItem(JetBrains.Util.IUserDataHolder cache)
        {
            return this;
        }

        /// <summary>Override to implement fix action.</summary>
        public abstract void Execute(ISolution solution, ITextControl textControl);

        /// <summary>Override to provide description of a fix.</summary>
        public abstract string Text { get; }
    }
}