using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal interface IExceptionsOrigin
    {
        List<ThrownExceptionModel> ThrownExceptions { get; }
        IBlockModel ContainingBlockModel { get; }
        bool Throws(IDeclaredType exceptionType);
        DocumentRange DocumentRange { get; }
    }
}