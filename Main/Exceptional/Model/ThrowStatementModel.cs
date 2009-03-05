using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class ThrowStatementModel : ModelBase
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
        public CatchClauseModel OuterCatchClauseModel { get; set; }

        private DocumentRange DocumentRange
        {
            get { return this.ThrowStatement.GetDocumentRange(); }
        }

        public ExceptionCreationModel ExceptionCreationModel { get; set; }

        public IDeclaredType ExceptionType
        {
            get
            {
                var exception = this.ThrowStatement.Exception;
                return exception == null ? this._exceptionType : exception.GetExpressionType() as IDeclaredType;
            }
            set { _exceptionType = value; }
        }

        public ThrowStatementModel(IThrowStatement throwStatement)
        {
            ThrowStatement = throwStatement;
            IsValid = false;
            IsCatched = false;
            IsDocumented = false;
            ThrowsWithInnerException = true;
            this.ExceptionCreationModel = new ExceptionCreationModel(this.ThrowStatement.Exception as IObjectCreationExpression);
        }

        public bool Throws(string exception)
        {
            if (this.ExceptionType == null) return false;

            //TODO: We must compare by full name!
            var fullName = this.ExceptionType.GetCLRName();
            var index = fullName.LastIndexOf('.');
            if(index >= 0)
                fullName = fullName.Substring(index + 1);

            return fullName.Equals(exception);
        }

        public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if (this.IsCatched == false && this.IsDocumented == false)
            {
                process.AddHighlighting(this.DocumentRange,
                    new ExceptionNotDocumentedHighlighting(this));
            }

            if(this.ThrowsWithInnerException == false)
            {
                process.AddHighlighting(this.DocumentRange,
                    new ThrowFromCatchWithNoInnerExceptionHighlighting(this));
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }
    }
}