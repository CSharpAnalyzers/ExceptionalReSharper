/// <copyright file="IExceptionsOriginModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal interface IExceptionsOriginModel
    {
        List<ThrownExceptionModel> ThrownExceptions { get; }
        IBlockModel ContainingBlockModel { get; }
        bool Throws(IDeclaredType exceptionType);
        DocumentRange DocumentRange { get; }
        void Accept(AnalyzerBase anayzeBase);
    }
}