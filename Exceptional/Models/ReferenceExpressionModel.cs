using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;

namespace ReSharper.Exceptional.Models
{
    internal class ReferenceExpressionModel : TreeElementModelBase<IReferenceExpression>, IExceptionsOriginModel
    {
        public ReferenceExpressionModel(IAnalyzeUnit analyzeUnit, IReferenceExpression invocationExpression, IBlockModel containingBlock)
            : base(analyzeUnit, invocationExpression)
        {
            ContainingBlock = containingBlock;
            ThrownExceptions = ThrownExceptionsReader.Read(AnalyzeUnit, this, Node);
        }

        public IEnumerable<ThrownExceptionModel> ThrownExceptions { get; set; }

        public IBlockModel ContainingBlock { get; private set; }

        public override DocumentRange DocumentRange
        {
            get { return Node.Reference.GetDocumentRange(); }
        }

        public bool Throws(IDeclaredType exceptionType)
        {
            return ThrownExceptions.Any(e => e.Throws(exceptionType));
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var thrownExceptionModel in ThrownExceptions)
                thrownExceptionModel.Accept(analyzerBase);
        }

        public bool SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            var containingStatement = Node.GetContainingStatement();
            if (containingStatement != null && containingStatement.LastChild != null)
            {
                var codeElementFactory = new CodeElementFactory(GetElementFactory());
                var exceptionVariableName = NameFactory.CatchVariableName(Node, exceptionType);
                var tryStatement = codeElementFactory.CreateTryStatement(exceptionType, exceptionVariableName);

                var spaces = GetElementFactory().CreateWhitespaces(Environment.NewLine);

                LowLevelModificationUtil.AddChildAfter(containingStatement.LastChild, spaces[0]);

                var block = codeElementFactory.CreateBlock(containingStatement);
                tryStatement.SetTry(block);

                containingStatement.ReplaceBy(tryStatement);
                return true; 
            }
            return false; 
        }
    }
}