using System;
using System.Xml;
using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class ExceptionDocumentationModel : ModelBase
    {
        /// <summary>Containing <see cref="IDocCommentBlockNode"/>.</summary>
        public IDocCommentBlockNode DocCommentBlockNode { get; private set; }

        /// <summary>Exception documentation xml node.</summary>
        public XmlNode ExceptionNode { get; private set; }

        /// <summary>Exception type name.</summary>
        public string ExceptionTypeName { get; private set; }

        /// <summary>Exception documentation tab <see cref="DocumentRange"/>.</summary>
        public DocumentRange DocumentRange { get; private set; }

        /// <summary>States that documented exception is actually thrown from documented method.</summary>
        public bool IsThrown { get; set; }

        public ExceptionDocumentationModel(IDocCommentBlockNode docCommentBlockNode, XmlNode exceptionNode) 
        {
            DocCommentBlockNode = docCommentBlockNode;
            ExceptionNode = exceptionNode;
            IsThrown = true;

            InitializeException();
            InitializeDocumentRange();
        }

        private void InitializeDocumentRange()
        {
            this.DocumentRange = XmlDocCommentHelper.FindRangeOfExceptionTag(this.ExceptionNode, this.ExceptionTypeName, this.DocCommentBlockNode);
        }

        private void InitializeException()
        {
            var attribute = this.ExceptionNode.Attributes["cref"];
            if (attribute == null) return;

            if (String.IsNullOrEmpty(attribute.Value)) return;

            this.ExceptionTypeName = attribute.Value;

            if (this.ExceptionTypeName.StartsWith("T:") || this.ExceptionTypeName.StartsWith("!:"))
            {
                this.ExceptionTypeName = this.ExceptionTypeName.Substring(2);
            }
        }

        public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if(this.IsThrown == false)
            {
                process.AddHighlighting(this.DocumentRange, new ExceptionNotThrownHighlighting(this));
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }
    }
}