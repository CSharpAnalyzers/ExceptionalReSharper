using System;
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    public class MethodContext
    {
        private readonly IMethodDeclaration _methodDeclaration;
        private readonly List<IModel> _models;
        private readonly Stack<ICatchClause> _catchClauseStack;
        private readonly Stack<List<IDeclaredType>> _tryBlockStack;
        private DocumentedExceptionsModel _documentedExceptionsModel;
        private readonly ThrownExceptionsModel _thrownExceptionsModel;

        internal IEnumerable<IModel> Result
        {
            get
            {
                foreach (var exception in this._thrownExceptionsModel.ThrownExceptions)
                {
                    yield return exception;
                }

                if (this._documentedExceptionsModel != null)
                {
                    foreach (var exception in this._documentedExceptionsModel.DocumentedExceptions)
                    {
                        yield return exception;
                    }
                }

                foreach (var exception in _models)
                {
                    yield return exception;
                }
            }
        }

        public MethodContext(IMethodDeclaration methodDeclaration)
        {
            this._methodDeclaration = methodDeclaration;
            this._models = new List<IModel>();

            this._tryBlockStack = new Stack<List<IDeclaredType>>();
            this._catchClauseStack = new Stack<ICatchClause>();

            this._thrownExceptionsModel = new ThrownExceptionsModel();
        }

        public bool IsDefinedFor(IMethodDeclaration methodDeclaration)
        {
            return this._methodDeclaration.Equals(methodDeclaration);
        }

        public void EnterTryBlock(ITryStatement tryStatement)
        {
            var newLevel = new List<IDeclaredType>();
            this._tryBlockStack.Push(newLevel);

            foreach (var catchClause in tryStatement.Catches)
            {
                newLevel.Add(catchClause.ExceptionType);
            }
        }

        public void LeaveTryBlock()
        {
            this._tryBlockStack.Pop();
        }

        public void EnterCatchClause(ICatchClause catchClause)
        {
            this._catchClauseStack.Push(catchClause);
        }

        public void LeaveCatchClause()
        {
            this._catchClauseStack.Pop();
        }

        public void Add(DocumentedExceptionsModel documentedExceptionsModel)
        {
            if (documentedExceptionsModel == null) return;

            if(this._documentedExceptionsModel != null)
                throw new InvalidOperationException("Processed method already has a documentation model.");

            this._documentedExceptionsModel = documentedExceptionsModel;

            var isThrownExceptionDocumentedExceptionAnalyzer = new IsThrownExceptionDocumentedExceptionAnalyzer(documentedExceptionsModel);
            this._thrownExceptionsModel.Accept(isThrownExceptionDocumentedExceptionAnalyzer);

            var isDocumentedExceptionThrownAnalyzer = new IsDocumentedExceptionThrownAnalyzer(this._thrownExceptionsModel);
            documentedExceptionsModel.Accept(isDocumentedExceptionThrownAnalyzer);
        }

        public void Add(ThrowStatementModel throwStatementModel)
        {
            if(throwStatementModel == null) return;

            var catchedVisitor = new IsThrownExceptionCatchedAnalyzer(this._tryBlockStack);
            throwStatementModel.Accept(catchedVisitor);

            var visitor = new HasInnerExceptionFromOuterCatchClauseAnalyzer(this._catchClauseStack);
            throwStatementModel.Accept(visitor);

            this._thrownExceptionsModel.ThrownExceptions.Add(throwStatementModel);
        }

        public void Add(CatchAllClauseModel catchAllClauseModel)
        {
            this._models.Add(catchAllClauseModel);
        }

        public void ComputeResult(CSharpErrorStageProcessBase process)
        {
            foreach (var model in this.Result)
            {
                if (model == null) continue;

                model.AssignHighlights(process);
            }
        }
    }
}