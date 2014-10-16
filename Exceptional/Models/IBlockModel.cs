using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharper.Exceptional.Models
{
    internal interface IBlockModel
    {
        List<IExceptionsOriginModel> ExceptionOriginModels { get; }
        List<TryStatementModel> TryStatementModels { get; }
        IBlockModel ParentBlock { get; }
        bool CatchesException(IDeclaredType exception);
        IDeclaredType GetCaughtException();
        IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCaught { get; }
        TryStatementModel FindNearestTryBlock();
        IBlock Contents { get; }
    }
}