using System.Collections.Generic;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal interface IBlockModel
    {
        List<ThrowStatementModel> ThrowStatementModels { get; }
        List<TryStatementModel> TryStatementModels { get; }
        IBlockModel ParentBlock { get; set; }
        bool CatchesException(IDeclaredType exception);
        IDeclaredType GetCatchedException();
        IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCatched { get; }
    }
}