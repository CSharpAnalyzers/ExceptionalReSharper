using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotDocumentedHighlighting : IHighlighting
    {
        public IThrowStatement ThrowStatement { get; private set; }

        public IDeclaredType Exception
        {
            get { return this.ThrowStatement.Exception.GetExpressionType() as IDeclaredType; }
        }

        public ExceptionNotDocumentedHighlighting(IThrowStatement throwStatement)
        {
            ThrowStatement = throwStatement;
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