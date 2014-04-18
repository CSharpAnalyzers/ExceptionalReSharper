// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal interface IExceptionsOriginModel
    {
        IEnumerable<ThrownExceptionModel> ThrownExceptions { get; }
        IBlockModel ContainingBlockModel { get; }
        bool Throws(IDeclaredType exceptionType);
        DocumentRange DocumentRange { get; }
        void Accept(AnalyzerBase anayzeBase);
        void SurroundWithTryBlock(IDeclaredType exceptionType);
    }
}