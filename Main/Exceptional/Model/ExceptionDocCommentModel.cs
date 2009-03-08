using System.Text.RegularExpressions;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ExceptionDocCommentModel : DocCommentModel
    {
        public IDeclaredType ExceptionType { get; set; }

        public ExceptionDocCommentModel(DocCommentBlockModel docCommentBlockModel) 
            : base(docCommentBlockModel) { }

        public override void Initialize()
        {
            base.Initialize();

            this.ExceptionType = GetExceptionType();
        }

        private IDeclaredType GetExceptionType()
        {
            var regEx = new Regex("cref=\"(?<exception>[^\"]+)\"");

            foreach (var docCommentNode in this.DocCommentNodes)
            {
                var text = docCommentNode.GetText();
                var match = regEx.Match(text);
                if (match.Success == false) continue;

                var exceptionType = match.Groups["exception"].Value;
                return TypeFactory.CreateTypeByCLRName(exceptionType, docCommentNode.GetProject());
            }

            return null;
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }

        protected override DocumentRange GetDocCommentRage()
        {
            var regEx = new Regex("cref=\"(?<exception>[^\"]+)\"");

            foreach (var docCommentNode in this.DocCommentNodes)
            {
                var text = docCommentNode.GetText();
                var match = regEx.Match(text);
                if (match.Success == false) continue;

                var exceptionType = match.Groups["exception"].Value;
                var documentRange = docCommentNode.GetDocumentRange();
                var textRange = documentRange.TextRange;
                var index = text.IndexOf("cref=\"");
                var startOffset = textRange.StartOffset + index + 6;
                var endOffset = startOffset + exceptionType.Length;
                var newTextRange = new TextRange(startOffset, endOffset);
                var newDocumentRange = new DocumentRange(documentRange.Document, newTextRange);

                return newDocumentRange;
            }

            return DocumentRange.InvalidRange;
        }
    }
}