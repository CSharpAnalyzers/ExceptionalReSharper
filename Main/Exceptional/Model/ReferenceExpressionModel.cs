// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.

using System;
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ReferenceExpressionModel : TreeElementModelBase<IReferenceExpression>, IExceptionsOriginModel
    {
        public IEnumerable<ThrownExceptionModel> ThrownExceptions { get; set; }
        public IBlockModel ContainingBlockModel { get; private set; }

        public ReferenceExpressionModel(IAnalyzeUnit analyzeUnit, IReferenceExpression invocationExpression,
                                        IBlockModel containingBlockModel)
            : base(analyzeUnit, invocationExpression)
        {
            ContainingBlockModel = containingBlockModel;
            containingBlockModel.ExceptionOriginModels.Add(this);

            ThrownExceptions = GetThrownExceptions();
        }

        private IEnumerable<ThrownExceptionModel> GetThrownExceptions()
        {
            var result = new List<ThrownExceptionModel>();

            foreach (var exceptionType in ThrownExceptionsReader.Read(Node))
            {
                var thrownException = new ThrownExceptionModel(
                    AnalyzeUnit, exceptionType, this);

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