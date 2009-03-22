/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ReferenceExpressionModel : TreeElementModelBase<IReferenceExpressionNode>, IExceptionsOriginModel
    {
        public List<ThrownExceptionModel> ThrownExceptions { get; set; }
        public IBlockModel ContainingBlockModel { get; private set; }

        public ReferenceExpressionModel(IAnalyzeUnit analyzeUnit, IReferenceExpressionNode invocationExpression,
                                        IBlockModel containingBlockModel)
            : base(analyzeUnit, invocationExpression)
        {
            ContainingBlockModel = containingBlockModel;
            containingBlockModel.ExceptionOriginModels.Add(this);

            ThrownExceptions = GetThrownExceptions();
        }

        private List<ThrownExceptionModel> GetThrownExceptions()
        {
            var result = new List<ThrownExceptionModel>();

            foreach (var exceptionType in ThrownExceptionsReader.Read(this.Node))
            {
                var thrownException = new ThrownExceptionModel(
                    this.AnalyzeUnit, exceptionType, this);

                result.Add(thrownException);
            }

            return result;
        }

        public bool Throws(IDeclaredType exceptionType)
        {
            foreach (var thrownExceptionModel in this.ThrownExceptions)
            {
                if (thrownExceptionModel.Throws(exceptionType))
                {
                    return true;
                }
            }

            return false;
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var thrownExceptionModel in this.ThrownExceptions)
            {
                thrownExceptionModel.Accept(analyzerBase);
            }
        }
    }
}