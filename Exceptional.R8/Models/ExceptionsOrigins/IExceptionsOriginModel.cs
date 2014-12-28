using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models.ExceptionsOrigins
{
    /// <summary>Describes a location where exceptions can be thrown. </summary>
    internal interface IExceptionsOriginModel
    {
        /// <summary>Gets the list of exception which can be thrown from this object. </summary>
        IEnumerable<ThrownExceptionModel> ThrownExceptions { get; }

        /// <summary>Gets the parent block which contains this block. </summary>
        IBlockModel ContainingBlock { get; }

        /// <summary>Gets the document range of this block. </summary>
        DocumentRange DocumentRange { get; }

        /// <summary>Gets the node. </summary>
        ITreeNode Node { get; }

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer. </param>
        void Accept(AnalyzerBase analyzer);

        /// <summary>Creates a try-catch block around this block. </summary>
        /// <param name="exceptionType">The exception type to catch. </param>
        /// <returns><c>true</c> if the try-catch block could be created; otherwise, <c>false</c>. </returns>
        bool SurroundWithTryBlock(IDeclaredType exceptionType);
    }
}