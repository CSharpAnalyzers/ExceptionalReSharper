using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class ThrowStatementModel : IModel
    {
        private IDeclaredType _exceptionType;
        public bool IsValid { get; private set; }
        public bool IsCatched { get; set; }
        public bool IsDocumented { get; set; }
        public bool ThrowsWithInnerException { get; set; }
        public bool IsRethrow
        {
            get { return this.ThrowStatement.Exception == null; }
        }

        public IThrowStatement ThrowStatement { get; set; }

        private DocumentRange DocumentRange
        {
            get { return this.ThrowStatement.GetDocumentRange(); }
        }

        public IDeclaredType ExceptionType
        {
            get
            {
                var exception = this.ThrowStatement.Exception;
                return exception == null ? this._exceptionType : exception.GetExpressionType() as IDeclaredType;
            }
            set { _exceptionType = value; }
        }

        private ThrowStatementModel(IThrowStatement throwStatement)
        {
            ThrowStatement = throwStatement;
            IsValid = false;
            IsCatched = false;
            IsDocumented = false;
            ThrowsWithInnerException = true;
        }

        public static ThrowStatementModel Create(IThrowStatement throwStatement)
        {
            var model = new ThrowStatementModel(throwStatement);

            return model;
        }

        public bool Throws(string exception)
        {
            if (this.ExceptionType == null) return false;

            return this.ExceptionType.GetCLRName().Equals(exception);
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if (this.IsCatched == false && this.IsDocumented == false)
            {
                process.AddHighlighting(this.DocumentRange,
                    new ExceptionNotDocumentedHighlighting(this.ThrowStatement));
            }

            if(this.ThrowsWithInnerException == false)
            {
                process.AddHighlighting(this.DocumentRange,
                    new ThrowFromCatchWithNoInnerExceptionHighlighting());
            }
        }

        public void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}