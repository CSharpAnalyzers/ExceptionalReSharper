using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class InvocationModel : ModelBase, IExceptionsOrigin
    {
        public IInvocationExpression InvocationExpression { get; private set; }

        public InvocationModel(MethodDeclarationModel methodDeclarationModel, IInvocationExpression invocationExpression, IBlockModel containingBlockModel)
            : base(methodDeclarationModel)
        {
            InvocationExpression = invocationExpression;
            ContainingBlockModel = containingBlockModel;
        }

        public List<ThrownExceptionModel> ThrownExceptions
        {
            //TODO: Implement
            get { return new List<ThrownExceptionModel>(); }
        }

        public IBlockModel ContainingBlockModel { get; private set; }

        public bool Throws(IDeclaredType exceptionType)
        {
            //TODO: Implement
            return false;
        }
    }
}