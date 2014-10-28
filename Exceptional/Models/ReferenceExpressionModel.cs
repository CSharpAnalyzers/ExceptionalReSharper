using System;
using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;

namespace ReSharper.Exceptional.Models
{
    internal class ReferenceExpressionModel : TreeElementModelBase<IReferenceExpression>, IExceptionsOriginModel
    {
        private IEnumerable<ThrownExceptionModel> _thrownExceptions;

        public ReferenceExpressionModel(IAnalyzeUnit analyzeUnit, IReferenceExpression invocationExpression, IBlockModel containingBlock)
            : base(analyzeUnit, invocationExpression)
        {
            ContainingBlock = containingBlock;
        }

        public IBlockModel ContainingBlock { get; private set; }

        public override DocumentRange DocumentRange
        {
            get { return Node.Reference.GetDocumentRange(); }
        }

        /// <summary>Gets a list of exceptions which may be thrown from this reference expression (empty if <see cref="IsInvocation"/> is false). </summary>
        public IEnumerable<ThrownExceptionModel> ThrownExceptions
        {
            get
            {
                if (_thrownExceptions == null)
                {
                    _thrownExceptions = IsInvocation
                        ? ThrownExceptionsReader.Read(AnalyzeUnit, this, Node)
                        : new List<ThrownExceptionModel>();
                }
                return _thrownExceptions;
            }
        }

        /// <summary>Gets a value indicating whether this is a method or property invocation. </summary>
        public bool IsInvocation
        {
            get
            {
                ITreeNode parent = Node.Parent;
                while (parent != null)
                {
                    if (parent == AnalyzeUnit.Node || parent is ICSharpArgument)
                        return false;

                    if (parent is IInvocationExpression || parent is IExpressionInitializer)
                        return true;

                    parent = parent.Parent;
                }
                return false;
            }
        }

        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var thrownExceptionModel in ThrownExceptions)
                thrownExceptionModel.Accept(analyzer);
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