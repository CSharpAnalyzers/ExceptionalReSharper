using System.Collections.Generic;
using System.Linq;
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
        private readonly Stack<ICatchClause> _catchClauseStack;
        private readonly Stack<List<IDeclaredType>> _tryBlockStack;

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

            this._tryBlockStack = new Stack<List<IDeclaredType>>();
            this._catchClauseStack = new Stack<ICatchClause>();
        }

        public bool IsDefinedFor(IMethodDeclaration methodDeclaration)
        {
            return this._methodDeclaration.Equals(methodDeclaration);
        }

        internal void AddThrownException(ThrowStatementModel throwStatement)
        {
            throwStatement.IsCatched = IsCatched(throwStatement.ThrowStatement);
            throwStatement.ContainsInnerException = ContainsOuterException(throwStatement.ThrowStatement);
            this._thrownExceptions.Add(throwStatement);
        }

        private bool ContainsOuterException(IThrowStatement throwStatement)
        {
            if(this._catchClauseStack.Count == 0) return true;

            var outerCatch = this._catchClauseStack.Peek() as ILocalScope;
            if(outerCatch == null) return false;

            //Catch clause with no named parameter
            if(outerCatch.LocalVariables.Count == 0) return false;

            var list = new List<IDeclaredElement>(outerCatch.LocalVariables);
            var catchVariable = list.Find(element => element is ICatchVariableDeclaration);
            if(catchVariable == null) return false;

            var exception = throwStatement.Exception as IObjectCreationExpressionNode;
            if (exception == null) return false;

            var arguments = new List<ICSharpArgumentNode>(exception.ArgumentList.Arguments);
            var match = arguments.Find(arg =>
                                           {
                                               var reference = arg.ValueNode as IReferenceExpressionNode;
                                               if (reference == null) return false;

                                               return reference.NameIdentifier.Name.Equals(catchVariable.ShortName);
                                           });

            return match != null;
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

            foreach (var list in this._tryBlockStack)
            {
                foreach (var type in list)
                {
                    if (type.Equals(exception))
                        return true;
                }
            }

            return false;
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

        public void AddModel(IModel model)
        {
            this._models.Add(model);
        }
    }
}