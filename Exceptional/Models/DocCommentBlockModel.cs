using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Stores data about processed <see cref="IDocCommentBlockNode"/>. </summary>
    internal class DocCommentBlockModel : TreeElementModelBase<IDocCommentBlockNode>
    {
        private string _documentationText;

        public DocCommentBlockModel(IAnalyzeUnit analyzeUnit, IDocCommentBlockNode docCommentNode)
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

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var exception in DocumentedExceptions)
                exception.Accept(analyzerBase);
        }

        public ExceptionDocCommentModel AddExceptionDocumentation(IDeclaredType exceptionType, string exceptionDescription, IProgressIndicator progress)
        {
            if (exceptionType == null)
                return null;
            
            var exceptionDocumentation = string.IsNullOrEmpty(exceptionDescription)
                ? string.Format("<exception cref=\"{0}\">" + Constants.ExceptionDescriptionMarker + ". </exception>{1}", exceptionType.GetClrName().ShortName, Environment.NewLine)
                : string.Format("<exception cref=\"{0}\">{1}</exception>{2}", exceptionType.GetClrName().ShortName, exceptionDescription, Environment.NewLine);

            ChangeDocumentation(_documentationText + "\n" + exceptionDocumentation);
            
            return DocumentedExceptions.LastOrDefault();
        }

        public void RemoveExceptionDocumentation(ExceptionDocCommentModel exceptionDocumentation, IProgressIndicator progress)
        {
            if (exceptionDocumentation == null)
                return;

            var regex = "<exception cref=\"" + Regex.Escape(exceptionDocumentation.ExceptionTypeName) + "\"(>((\r|\n|.)*?)</exception>)?";
            var newDocumentation = Regex.Replace(_documentationText, regex, string.Empty);
            ChangeDocumentation(newDocumentation);
        }

        private void ChangeDocumentation(string text)
        {
            var newNode = GetElementFactory().CreateDocCommentBlock(text.Trim('\n').Trim('\r'));

            //Shell.Instance.GetComponent<IShellLocks>().AcquireWriteLock();
            //try
            //{
            //}
            //finally
            //{
            //    Shell.Instance.GetComponent<IShellLocks>().ReleaseWriteLock();
            //}

            if (IsCreated)
                LowLevelModificationUtil.ReplaceChildRange(Node, Node, newNode);
            else
                LowLevelModificationUtil.AddChildBefore(AnalyzeUnit.Node.FirstChild, newNode);
            newNode.FormatNode();

            Node = newNode;
            Update();
        }

        private void Update()
        {
            if (!IsCreated)
                return;

            _documentationText = GetDocumentationXml();

            References = new List<IReference>(Node.GetFirstClassReferences());
            DocumentedExceptions = GetDocumentedExceptions();
        }

        private IEnumerable<ExceptionDocCommentModel> GetDocumentedExceptions()
        {
            var regex = new Regex("<exception cref=\"(.*?)\"(>((\r|\n|.)*?)</exception>)?");
            var exceptions = new List<ExceptionDocCommentModel>();
            foreach (Match match in regex.Matches(_documentationText))
            {
                var exceptionType = match.Groups[1].Value;
                var exceptionDescription = match.Groups[3].Value;

                exceptions.Add(new ExceptionDocCommentModel(this, exceptionType, exceptionDescription));
            }
            return exceptions;
        }

        private string GetDocumentationXml()
        {
            var xml = string.Empty;
            foreach (var node in Node.Children().OfType<IDocCommentNode>())
                xml += node.GetText().Replace("///", "").Trim() + "\n";
            return xml;
        }
    }
}