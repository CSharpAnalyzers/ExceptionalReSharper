// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal interface IBlockModel
    {
        List<IExceptionsOriginModel> ExceptionOriginModels { get; }
        List<TryStatementModel> TryStatementModels { get; }
        IBlockModel ParentBlock { get; }
        bool CatchesException(IDeclaredType exception);
        IDeclaredType GetCatchedException();
        IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCatched { get; }
        TryStatementModel FindNearestTryBlock();
        IBlock Contents { get; }
    }
}