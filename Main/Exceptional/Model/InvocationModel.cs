/// <copyright file="InvocationModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class InvocationModel : TreeElementModelBase<IInvocationExpressionNode>, IExceptionsOrigin
    {
        public IBlockModel ContainingBlockModel { get; private set; }

        public InvocationModel(IAnalyzeUnit analyzeUnit, IInvocationExpressionNode invocationExpression, IBlockModel containingBlockModel)
            : base(analyzeUnit, invocationExpression)
        {
            ContainingBlockModel = containingBlockModel;
        }

        public List<ThrownExceptionModel> ThrownExceptions
        {
            //TODO: Implement
            get { return new List<ThrownExceptionModel>(); }
        }

        public bool Throws(IDeclaredType exceptionType)
        {
            //TODO: Implement
            return false;
        }
    }
}