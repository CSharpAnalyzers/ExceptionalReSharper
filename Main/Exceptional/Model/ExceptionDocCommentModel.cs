// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System.Text.RegularExpressions;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ExceptionDocCommentModel : DocCommentModel
    {
        public IDeclaredType ExceptionType { get; private set; }

        public ExceptionDocCommentModel(DocCommentBlockModel docCommentBlockModel)
            : base(docCommentBlockModel)
        {
        }

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

                var exceptionReference =
                    this.DocCommentBlockModel.References.Find(reference => reference.GetName().Equals(exceptionType));
                if (exceptionReference == null)
                {
                    return TypeFactory.CreateTypeByCLRName(exceptionType, docCommentNode.GetPsiModule());
                }

                var resolveResult = exceptionReference.Resolve();
                var declaredType = resolveResult.DeclaredElement as ITypeElement;
                if (declaredType == null)
                {
                    return TypeFactory.CreateTypeByCLRName(exceptionType, docCommentNode.GetPsiModule());
                }

                return TypeFactory.CreateTypeByCLRName(declaredType.CLRName, docCommentNode.GetPsiModule());
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

        public DocumentRange GetDescriptionDocumentRange()
        {
            foreach (var docCommentNode in this.DocCommentNodes)
            {
                var text = docCommentNode.GetText();
                if (text.Contains("[MARKER]") == false) continue;

                var documentRange = docCommentNode.GetDocumentRange();
                var textRange = documentRange.TextRange;
                var index = text.IndexOf("[MARKER]");
                var startOffset = textRange.StartOffset + index;
                var endOffset = startOffset + 8;
                var newTextRange = new TextRange(startOffset, endOffset);
                var newDocumentRange = new DocumentRange(documentRange.Document, newTextRange);

                return newDocumentRange;
            }

            return DocumentRange.InvalidRange;
        }
    }
}