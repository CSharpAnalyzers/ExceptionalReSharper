// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
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
    }
}