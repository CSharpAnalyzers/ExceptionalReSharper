using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Describes a location where an exception has been thrown. </summary>
    internal interface IExceptionsOriginModel
    {
        /// <summary>Gets the list of exception which can be thrown from this object. </summary>
        IEnumerable<ThrownExceptionModel> ThrownExceptions { get; }

        /// <summary>Gets the parent block which contains this block. </summary>
        IBlockModel ContainingBlock { get; }

        /// <summary>Checks whether this object can throw the given exception type. </summary>
        /// <param name="exceptionType">The exception type. </param>
        /// <returns><c>true</c> if the object may be throwing the given exception type; otherwise, <c>false</c>. </returns>
        bool Throws(IDeclaredType exceptionType);
        
        DocumentRange DocumentRange { get; }
        
        void Accept(AnalyzerBase analyzeBase);
        
        void SurroundWithTryBlock(IDeclaredType exceptionType);
    }
}