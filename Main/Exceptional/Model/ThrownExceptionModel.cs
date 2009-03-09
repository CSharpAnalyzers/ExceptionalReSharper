using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ThrownExceptionModel : ModelBase
    {
        public bool IsCatched { get; private set; }
        public bool IsDocumented { get; private set; }

        public IDeclaredType ExceptionType { get; private set; }
        public IExceptionsOrigin Parent { get; private set; }

        public override DocumentRange DocumentRange
        {
            get { return this.Parent.DocumentRange; }
        }

        public ThrownExceptionModel(MethodDeclarationModel methodDeclarationModel, IDeclaredType exceptionType, IExceptionsOrigin parent) : base(methodDeclarationModel)
        {
            ExceptionType = exceptionType;
            Parent = parent;

            IsCatched = CheckIfCatched();
            IsDocumented = CheckIfDocumented();
        }

        private bool CheckIfDocumented()
        {
            var docCommentBlockNode = this.MethodDeclarationModel.DocCommentBlockModel;
            if (docCommentBlockNode == null) return false;

            foreach (var exceptionDocumentationModel in docCommentBlockNode.ExceptionDocCommentModels)
            {
                if (this.Throws(exceptionDocumentationModel.ExceptionType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckIfCatched()
        {
            if (ExceptionType == null) return false;

            return this.Parent.ContainingBlockModel.CatchesException(ExceptionType);
        }

        /// <summary>Checks whether this thrown exception equals to <paramref name="exceptionType"/>.</summary>
        public bool Throws(IDeclaredType exceptionType)
        {
            if (this.ExceptionType == null) return false;
            if (exceptionType == null) return false;

            return this.ExceptionType.GetCLRName().Equals(exceptionType.GetCLRName());
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }
    }
}