using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Models.ExceptionsOrigins;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Stores data about processed <see cref="IDocCommentBlockNode"/>. </summary>
    internal class DocCommentBlockModel : TreeElementModelBase<IDocCommentBlock>
    {
        private string _documentationText;

        public DocCommentBlockModel(IAnalyzeUnit analyzeUnit, IDocCommentBlock docCommentNode)
            : base(analyzeUnit, docCommentNode)
        {
            References = new List<IReference>();
            DocumentedExceptions = new List<ExceptionDocCommentModel>();
            Update();
        }

        public bool IsCreated
        {
            get { return Node != null; }
        }

        public List<IReference> References { get; private set; }

        public IEnumerable<ExceptionDocCommentModel> DocumentedExceptions { get; private set; }

        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var exception in DocumentedExceptions)
                exception.Accept(analyzer);
        }

        public ExceptionDocCommentModel AddExceptionDocumentation(ThrownExceptionModel thrownException, IProgressIndicator progressIndicator)
        {
            if (thrownException.ExceptionType == null)
                return null;

            var exceptionDescription = thrownException.ExceptionDescription.Trim();

            if (thrownException.ExceptionsOrigin is ThrowStatementModel)
            {
                if (thrownException.ExceptionType.GetClrName().FullName == "System.ArgumentNullException")
                    exceptionDescription = ArgumentNullExceptionDescription.CreateFrom(thrownException).GetDescription().Trim();
            }
            else
                exceptionDescription = Regex.Replace(exceptionDescription, "<paramref name=\"(.*?)\"/>", m => m.Groups[1].Value).Trim();

            var exceptionDocumentation = string.IsNullOrEmpty(exceptionDescription)
                ? string.Format("<exception cref=\"T:{0}\">" + Constants.ExceptionDescriptionMarker + ".</exception>{1}", thrownException.ExceptionType.GetClrName().FullName, Environment.NewLine)
                : string.Format("<exception cref=\"T:{0}\">{1}</exception>{2}", thrownException.ExceptionType.GetClrName().FullName, exceptionDescription, Environment.NewLine);

            if (thrownException.ExceptionsOrigin.ContainingBlock is AccessorDeclarationModel)
            {
                var accessor = ((AccessorDeclarationModel)thrownException.ExceptionsOrigin.ContainingBlock).Node.NameIdentifier.Name;
                exceptionDocumentation = string.IsNullOrEmpty(exceptionDescription)
                    ? string.Format("<exception cref=\"T:{0}\" accessor=\"{1}\">" + Constants.ExceptionDescriptionMarker + ".</exception>{2}", thrownException.ExceptionType.GetClrName().FullName, accessor, Environment.NewLine)
                    : string.Format("<exception cref=\"T:{0}\" accessor=\"{1}\">{2}</exception>{3}", thrownException.ExceptionType.GetClrName().FullName, accessor, exceptionDescription, Environment.NewLine);
            }

            ChangeDocumentation(_documentationText + "\n" + exceptionDocumentation);

            return DocumentedExceptions.LastOrDefault();
        }

        public void RemoveExceptionDocumentation(ExceptionDocCommentModel exceptionDocumentation, IProgressIndicator progress)
        {
            if (exceptionDocumentation == null)
                return;

            var attributes = "cref=\"" + Regex.Escape(exceptionDocumentation.ExceptionTypeName) + "\"";
            if (exceptionDocumentation.Accessor != null)
                attributes += " accessor=\"" + Regex.Escape(exceptionDocumentation.Accessor) + "\"";

            var regex = "<exception " + attributes + "((>((\r|\n|.)*?)</exception>)|((\r|\n|.)*?/>))";
            var newDocumentation = Regex.Replace(_documentationText, regex, string.Empty);
            ChangeDocumentation(newDocumentation);
        }

        private void ChangeDocumentation(string text)
        {
            text = text.Trim('\r', '\n');

            if (!string.IsNullOrEmpty(text))
            {
                var newNode = GetElementFactory().CreateDocCommentBlock(text);
                if (IsCreated)
                    LowLevelModificationUtil.ReplaceChildRange(Node, Node, newNode);
                else
                    LowLevelModificationUtil.AddChildBefore(AnalyzeUnit.Node.FirstChild, newNode);
                newNode.FormatNode();

                Node = newNode;
            }
            else if (Node != null)
            {
                LowLevelModificationUtil.DeleteChild(Node);
                Node = null;
            }

            Update();
        }

        private void Update()
        {
            if (!IsCreated)
            {
                _documentationText = string.Empty;

                References = new List<IReference>();
                DocumentedExceptions = new List<ExceptionDocCommentModel>();
            }
            else
            {
                _documentationText = GetDocumentationXml();

                References = new List<IReference>(Node.GetFirstClassReferences());
                DocumentedExceptions = GetDocumentedExceptions();
            }
        }

        private IEnumerable<ExceptionDocCommentModel> GetDocumentedExceptions()
        {
            var regex = new Regex("<exception cref=\"(.*?)\"( accessor=\"(.*?)\")?(>((\r|\n|.)*?)</exception>)?");
            var exceptions = new List<ExceptionDocCommentModel>();
            foreach (Match match in regex.Matches(_documentationText))
            {
                var exceptionType = match.Groups[1].Value;
                var accessor = !string.IsNullOrEmpty(match.Groups[3].Value) ? match.Groups[3].Value : null;
                var exceptionDescription = match.Groups[5].Value;

                exceptions.Add(new ExceptionDocCommentModel(this, exceptionType, exceptionDescription, accessor));
            }
            return exceptions;
        }

        private string GetDocumentationXml()
        {
            var xml = string.Empty;
            foreach (var node in Node.Children().OfType<IDocCommentNode>())
                xml += node.GetText().Replace("/// ", "").Replace("///", "") + "\n";
            return xml;
        }
    }
}