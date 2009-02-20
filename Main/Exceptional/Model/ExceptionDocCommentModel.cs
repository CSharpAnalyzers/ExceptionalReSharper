using System;
using System.Xml;
using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class ExceptionDocCommentModel : IModel
    {
        public bool IsValid { get; private set; }
        public bool IsDocumentedExceptionThrown { get; set; }
        public string ExceptionType { get; private set; }

        private XmlNode Node { get; set; }
        private IMethodDeclaration MethodDeclaration { get; set; }
        private IDocCommentBlockNode DocCommentNode { get; set; }

        private DocumentRange DocumentRange 
        {
            get { return XmlDocCommentHelper.FindRangeOfExceptionTag(this.Node, this.ExceptionType, this.DocCommentNode); }
        }

        private ExceptionDocCommentModel(XmlNode node, IMethodDeclaration methodDeclaration)
        {
            Node = node;
            MethodDeclaration = methodDeclaration;
            IsValid = false;
            IsDocumentedExceptionThrown = true;
        }

        public static ExceptionDocCommentModel Create(XmlNode node, IMethodDeclaration methodDeclaration)
        {
            var model = new ExceptionDocCommentModel(node, methodDeclaration);
            model.Initialize();

            return model;
        }

        private void Initialize()
        {
            var docCommentOwner = this.MethodDeclaration.ToTreeNode() as IDocCommentBlockOwnerNode;
            if (docCommentOwner == null) return;

            this.DocCommentNode = docCommentOwner.GetDocCommentBlockNode();
            if (this.DocCommentNode == null) return;

            var attribute = this.Node.Attributes["cref"];
            if (attribute == null) return;

            if (String.IsNullOrEmpty(attribute.Value)) return;

            this.ExceptionType = attribute.Value;

            if (this.ExceptionType.StartsWith("T:"))
                this.ExceptionType = this.ExceptionType.Substring(2);

            this.IsValid = true;
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if (this.IsDocumentedExceptionThrown == false)
            {
                process.AddHighlighting(this.DocumentRange,
                    new ExceptionNotThrownHighlighting(this.ExceptionType));
            }
        }

        public void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}