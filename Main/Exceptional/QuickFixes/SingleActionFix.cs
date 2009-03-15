/// <copyright file="SingleActionFix.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.QuickFixes
{
    /// <summary>Base class for all fixes that serves only one <see cref="IBulbItem"/>.</summary>
    internal abstract class SingleActionFix : BulbItemImpl, IQuickFix
    {
        public bool IsAvailable(IUserDataHolder cache)
        {
            return true;
        }

        public IBulbItem[] Items
        {
            get { return new[] {this}; }
        }
    }
}