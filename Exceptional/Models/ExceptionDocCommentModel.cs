using System;
using System.Text.RegularExpressions;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    internal class ExceptionDocCommentModel : DocCommentModel
    {
        public IDeclaredType ExceptionType { get; private set; }
        public string ExceptionDescription { get; private set; }

        public ExceptionDocCommentModel(DocCommentBlockModel docCommentBlockModel)
            : base(docCommentBlockModel)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            GetExceptionType();
        }

        private void GetExceptionType()
        {
            var regex = new Regex("<exception cref=\"(.*?)\"(>(.*?)</exception>)?");
            foreach (var docCommentNode in DocCommentNodes)
            {
                var text = docCommentNode.GetText();
                var match = regex.Match(text);
                if (match.Success)
                {
                    var exceptionType = match.Groups[1].Value;

                    if (match.Groups.Count == 4)
                        ExceptionDescription = match.Groups[3].Value;

                    var exceptionReference = DocCommentBlockModel.References
                        .Find(reference => reference.GetName().Equals(exceptionType));
                    var psiModule = docCommentNode.GetPsiModule();

                    if (exceptionReference == null)
                        ExceptionType = TypeFactory.CreateTypeByCLRName(exceptionType, psiModule, psiModule.GetContextFromModule());
                    else
                    {
                        var resolveResult = exceptionReference.Resolve();
                        var declaredType = resolveResult.DeclaredElement as ITypeElement;
                        if (declaredType == null)
                            ExceptionType = TypeFactory.CreateTypeByCLRName(exceptionType, psiModule, psiModule.GetContextFromModule());
                        else
                            ExceptionType = TypeFactory.CreateType(declaredType);
                    }
                }
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }

        protected override DocumentRange GetDocCommentRage()
        {
            var regex = new Regex("<exception cref=\"(.*?)\"");
            foreach (var docCommentNode in DocCommentNodes)
            {
                var text = docCommentNode.GetText();

                var match = regex.Match(text);
                if (match.Success)
                {
                    var exceptionType = match.Groups[1].Value;
                    var documentRange = docCommentNode.GetDocumentRange();
                    var textRange = documentRange.TextRange;

                    var index = text.IndexOf("cref=\"", StringComparison.InvariantCulture);

                    var startOffset = textRange.StartOffset + index + 6;
                    var endOffset = startOffset + exceptionType.Length;

                    var newTextRange = new TextRange(startOffset, endOffset);
                    return new DocumentRange(documentRange.Document, newTextRange);
                }
            }

            return DocumentRange.InvalidRange;
        }

        public DocumentRange GetDescriptionDocumentRange()
        {
            foreach (var docCommentNode in DocCommentNodes)
            {
                var text = docCommentNode.GetText();
                if (text.Contains("[MARKER]"))
                {
                    var documentRange = docCommentNode.GetDocumentRange();
                    var textRange = documentRange.TextRange;

                    var index = text.IndexOf("[MARKER]", StringComparison.InvariantCulture);
                    var startOffset = textRange.StartOffset + index;
                    var endOffset = startOffset + 8;

                    var newTextRange = new TextRange(startOffset, endOffset);
                    return new DocumentRange(documentRange.Document, newTextRange);
                }
            }

            return DocumentRange.InvalidRange;
        }
    }
}