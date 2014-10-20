using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.Exceptional.Models
{
    internal interface IBlockModel
    {
        /// <summary>Gets the list of exception which can be thrown from this block. </summary>
        List<IExceptionsOriginModel> ThrownExceptions { get; }

        /// <summary>Gets the try statements defined in the block. </summary>
        List<TryStatementModel> TryStatements { get; }

        /// <summary>Gets the parent block. </summary>
        IBlockModel ParentBlock { get; }

        /// <summary>Checks whether the block catches the given exception. </summary>
        /// <param name="exception">The exception. </param>
        /// <returns><c>true</c> if the exception is caught in the block; otherwise, <c>false</c>. </returns>
        bool CatchesException(IDeclaredType exception);

        /// <summary>Gets the exception which is caught by the block. </summary>
        IDeclaredType CaughtException { get; }

        /// <summary>Gets the list of not caught thrown exceptions. </summary>
        IEnumerable<ThrownExceptionModel> NotCaughtThrownExceptions { get; }

        /// <summary>Finds the nearest parent try statement which encloses this block. </summary>
        /// <returns>The try statement. </returns>
        TryStatementModel FindNearestTryStatement();

        /// <summary>Gets the content block of the object. </summary>
        IBlock Contents { get; }
    }
}