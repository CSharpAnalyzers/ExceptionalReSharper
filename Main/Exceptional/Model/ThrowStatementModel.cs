using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ThrowStatementModel : IModel
    {
        public bool IsValid { get; private set; }
        public bool IsCatched { get; set; }
        public bool IsDocumented { get; set; }
        public bool ContainsInnerException { get; set; }

        public IThrowStatement ThrowStatement { get; set; }

        private DocumentRange DocumentRange
        {
            get { return this.ThrowStatement.GetDocumentRange(); }
        }

        private ThrowStatementModel(IThrowStatement throwStatement)
        {
            ThrowStatement = throwStatement;
            IsValid = false;
            IsCatched = false;
            IsDocumented = false;
        }

        public static ThrowStatementModel Create(IThrowStatement throwStatement)
        {
            var model = new ThrowStatementModel(throwStatement);
            model.Initialize();

            return model;
        }

        private void Initialize()
        {
            if (this.ThrowStatement.Exception == null) return;

            this.IsValid = true;
        }

        public bool Throws(string exception)
        {
            var exceptionType = this.ThrowStatement.Exception.GetExpressionType() as IDeclaredType;
            if (exceptionType == null) return false;

            return exceptionType.GetCLRName().Equals(exception);
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if (this.IsCatched == false && this.IsDocumented == false)
            {
                process.AddHighlighting(this.DocumentRange,
                    new ExceptionNotDocumentedHighlighting(this.ThrowStatement));
            }

            if(this.ContainsInnerException == false)
            {
                process.AddHighlighting(this.DocumentRange,
                    new ThrowFromCatchWithNoInnerExceptionHighlighting());
            }
        }
    }
}