using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotDocumentedHighlighting : IHighlighting
    {
        private readonly IThrowStatement _throwStatement;

        private IDeclaredType Exception
        {
            get { return this._throwStatement.Exception.GetExpressionType() as IDeclaredType; }
        }

        public ExceptionNotDocumentedHighlighting(IThrowStatement throwStatement)
        {
            _throwStatement = throwStatement;
        }

        public string ToolTip
        {
            get { return Message; }
        }

        public string ErrorStripeToolTip
        {
            get { return Message; }
        }

        private string Message
        {
            get
            {
                if (this.Exception == null)
                    throw new InvalidOperationException("The given exception was null.");

                return String.Format(Resources.HighLightNotDocumentedExceptions, this.Exception.GetCLRName());
            }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}