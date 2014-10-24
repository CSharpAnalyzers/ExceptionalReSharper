using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Describes a location where exceptions can be thrown. </summary>
    internal interface IExceptionsOriginModel
    {
        /// <summary>Gets the list of exception which can be thrown from this object. </summary>
        IEnumerable<ThrownExceptionModel> ThrownExceptions { get; }

        /// <summary>Gets the parent block which contains this block. </summary>
        IBlockModel ContainingBlock { get; }

        DocumentRange DocumentRange { get; }

        void Accept(AnalyzerBase analyzer);

        bool SurroundWithTryBlock(IDeclaredType exceptionType);
    }
}