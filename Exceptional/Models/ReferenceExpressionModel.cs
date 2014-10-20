using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;

namespace ReSharper.Exceptional.Models
{
    internal class ReferenceExpressionModel : TreeElementModelBase<IReferenceExpression>, IExceptionsOriginModel
    {
        public IEnumerable<ThrownExceptionModel> ThrownExceptions { get; set; }
        public IBlockModel ContainingBlock { get; private set; }

        public ReferenceExpressionModel(IAnalyzeUnit analyzeUnit, IReferenceExpression invocationExpression, IBlockModel containingBlock)
            : base(analyzeUnit, invocationExpression)
        {
            ContainingBlock = containingBlock;
            ThrownExceptions = GetThrownExceptions();
        }

        private IEnumerable<ThrownExceptionModel> GetThrownExceptions()
        {
            var result = new List<ThrownExceptionModel>();

            foreach (var exception in ThrownExceptionsReader.Read(Node))
            {
                var thrownException = new ThrownExceptionModel(
                    AnalyzeUnit, exception.ExceptionType, exception.ExceptionDescription, this);
                result.Add(thrownException);
            }

            return result;
        }

        public bool Throws(IDeclaredType exceptionType)
        {
            foreach (var thrownExceptionModel in ThrownExceptions)
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
            foreach (var thrownExceptionModel in ThrownExceptions)
            {
                thrownExceptionModel.Accept(analyzerBase);
            }
        }

        public void SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            var codeElementFactory = new CodeElementFactory(GetElementFactory());
            var exceptionVariableName = NameFactory.CatchVariableName(Node, exceptionType);
            var tryStatement = codeElementFactory.CreateTryStatement(exceptionType, exceptionVariableName);
            var containingStatement = Node.GetContainingStatement();
        	var spaces = GetElementFactory().CreateWhitespaces(Environment.NewLine);
			LowLevelModificationUtil.AddChildAfter(containingStatement.LastChild, spaces[0]);

            var block = codeElementFactory.CreateBlock(containingStatement);
			tryStatement.SetTry(block);
            
            containingStatement.ReplaceBy(tryStatement);
        }
    }
}