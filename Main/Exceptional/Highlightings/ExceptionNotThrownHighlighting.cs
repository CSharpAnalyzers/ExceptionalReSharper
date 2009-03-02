using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Highlightings
{
    [StaticSeverityHighlighting(Severity.WARNING)]
    public class ExceptionNotThrownHighlighting : IHighlighting
    {
        public ICSharpTypeMemberDeclaration MemberDeclaration { get; private set; }
        public DocumentRange DocumentRange { get; private set; }
        private readonly string _excetionType;

        public ExceptionNotThrownHighlighting(ICSharpTypeMemberDeclaration memberDeclaration, DocumentRange documentRange, string exceptionType)
        {
            MemberDeclaration = memberDeclaration;
            DocumentRange = documentRange;
            _excetionType = exceptionType;
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
            get { return String.Format(Resources.HighLightNotThrownDocumentedExceptions, _excetionType); }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}