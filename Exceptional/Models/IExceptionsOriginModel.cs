using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
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