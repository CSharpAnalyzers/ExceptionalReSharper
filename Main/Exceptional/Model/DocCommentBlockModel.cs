using System.Collections.Generic;
using System.Xml;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IDocCommentBlockNode"/>.</summary>
    public class DocCommentBlockModel : ModelBase
    {
        public IDocCommentBlockNode DocCommentNode { get; set; }
        public List<ExceptionDocumentationModel> Exceptions { get; private set; }

        public DocCommentBlockModel(IDocCommentBlockNode docCommentNode)
        {
            DocCommentNode = docCommentNode;
            this.Exceptions = new List<ExceptionDocumentationModel>();

            InitializeExceptionsDocumentation();
        }

        private void InitializeExceptionsDocumentation()
        {
            var xmlNode = this.Parent.MethodDeclaration.GetXMLDoc(false);
            if (xmlNode == null) return;

            var exceptionNodeList = xmlNode.SelectNodes("exception");
            if (exceptionNodeList == null || exceptionNodeList.Count == 0) return;

            foreach (XmlNode exceptionNode in exceptionNodeList)
            {
                var model = new ExceptionDocumentationModel(this.DocCommentNode, exceptionNode);
                this.Exceptions.Add(model);
            }
        }

        public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            foreach (var exceptionDocumentationModel in this.Exceptions)
            {
                exceptionDocumentationModel.AssignHighlights(process);
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            foreach (var exceptionDocumentationModel in this.Exceptions)
            {
                analyzerBase.Visit(exceptionDocumentationModel);
            }
        }
    }
}