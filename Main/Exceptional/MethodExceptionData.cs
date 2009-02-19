using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    public class MethodExceptionData
    {
        private readonly IMethodDeclaration _methodDeclaration;
        private readonly List<ThrowStatementModel> _thrownExceptions;
        private readonly List<ExceptionDocCommentModel> _documentedExceptions;
        private readonly List<IModel> _models;

        private readonly List<List<IDeclaredType>> _tryStack;

        private List<IDeclaredType> TryStackTop
        {
            get { return this._tryStack[this._tryStack.Count - 1]; }
        }

        internal IEnumerable<IModel> Result
        {
            get
            {
                foreach (var exception in _thrownExceptions)
                {
                    yield return exception;
                }

                foreach (var exception in _documentedExceptions)
                {
                    yield return exception;
                }

                foreach (var exception in _models)
                {
                    yield return exception;
                }
            }
        }

        public MethodExceptionData(IMethodDeclaration methodDeclaration)
        {
            this._methodDeclaration = methodDeclaration;
            this._thrownExceptions = new List<ThrowStatementModel>();
            this._documentedExceptions = new List<ExceptionDocCommentModel>();
            this._models = new List<IModel>();
            this._tryStack = new List<List<IDeclaredType>>();
        }

        public bool IsDefinedFor(IMethodDeclaration methodDeclaration)
        {
            return this._methodDeclaration.Equals(methodDeclaration);
        }

        internal void AddThrownException(ThrowStatementModel throwStatement)
        {
            throwStatement.IsCatched = IsCatched(throwStatement.ThrowStatement);
            this._thrownExceptions.Add(throwStatement);
        }

        private bool IsExceptionThrownOutside(string exception)
        {
            foreach (var throwStatement in _thrownExceptions)
            {
                if (throwStatement.Throws(exception) == false) continue;

                if (throwStatement.IsCatched) continue;

                return true;
            }

            return false;
        }

        internal void AddDocumentedException(ExceptionDocCommentModel exceptionDocCommentModel)
        {
            exceptionDocCommentModel.IsDocumentedExceptionThrown = this.IsExceptionThrownOutside(exceptionDocCommentModel.ExceptionType);

            //TODO: Check for duplicates
            this._documentedExceptions.Add(exceptionDocCommentModel);

            foreach (var throwStatement in _thrownExceptions)
            {
                if (throwStatement.Throws(exceptionDocCommentModel.ExceptionType))
                {
                    throwStatement.IsDocumented = true;
                }
            }
        }

        private bool IsCatched(IThrowStatement throwStatement)
        {
            var exception = throwStatement.Exception.GetExpressionType() as IDeclaredType;
            if (exception == null) return false;

            foreach (var list in _tryStack)
            {
                foreach (var type in list)
                {
                    if (type.Equals(exception))
                        return true;
                }
            }

            return false;
        }

        public void BeginTry(ITryStatement tryStatement)
        {
            this._tryStack.Add(new List<IDeclaredType>());

            foreach (var catchClause in tryStatement.Catches)
            {
                this.TryStackTop.Add(catchClause.ExceptionType);
            }
        }

        public void EndTry(ITryStatement tryStatement)
        {
            this._tryStack.Remove(this.TryStackTop);
        }

        public void AddModel(IModel model)
        {
            this._models.Add(model);
        }
    }
}