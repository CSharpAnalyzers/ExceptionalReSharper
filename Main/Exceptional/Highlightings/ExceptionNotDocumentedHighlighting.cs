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
        private const string MESSAGE = "The '{0}' exception is not documented in xml documentation.";

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
            get { return GetMessage(); }
        }

        public string ErrorStripeToolTip
        {
            get { return GetMessage(); }
        }

        private string GetMessage()
        {
            if (this.Exception == null)
                throw new InvalidOperationException("The given exception was null.");

            return String.Format(MESSAGE, this.Exception.GetCLRName());
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}