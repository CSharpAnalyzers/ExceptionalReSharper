using System;
using System.Collections.Generic;
using System.Xml;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IDocCommentBlockNode"/>.</summary>
    internal class DocCommentBlockModel : ModelBase
    {
        public IDocCommentBlockNode DocCommentNode { get; set; }
        public List<ExceptionDocumentationModel> Exceptions { get; private set; }
        private XmlNode DocCommentXmlNode { get; set; }

        public DocCommentBlockModel(MethodDeclarationModel methodDeclarationModel, IDocCommentBlockNode docCommentNode) : base(methodDeclarationModel)
        {
            DocCommentNode = docCommentNode;
            this.Exceptions = new List<ExceptionDocumentationModel>();

            //InitializeExceptionsDocumentation();
        }

//        private void InitializeExceptionsDocumentation()
//        {
//            this.DocCommentXmlNode = this.MethodDeclarationModel.MethodDeclaration.GetXMLDoc(false);
//            if (this.DocCommentXmlNode == null) return;
//
//            var exceptionNodeList = this.DocCommentXmlNode.SelectNodes("exception");
//            if (exceptionNodeList == null || exceptionNodeList.Count == 0) return;
//
//            foreach (XmlNode exceptionNode in exceptionNodeList)
//            {
//                var model = new ExceptionDocumentationModel(this.MethodDeclarationModel, this, exceptionNode);
//                this.Exceptions.Add(model);
//            }
//        }

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
                exceptionDocumentationModel.Accept(analyzerBase);
            }
        }

        public void Remove(ExceptionDocumentationModel exceptionDocumentationModel)
        {
            this.Exceptions.Remove(exceptionDocumentationModel);

            var comment = String.Empty;
            foreach (XmlNode node in this.DocCommentXmlNode.ChildNodes)
            {
                if(node.Name.Equals("exception")) continue;

                comment += "/// " + node.OuterXml + Environment.NewLine;
            }

            foreach (var exception in this.Exceptions)
            {
                //comment += "/// " + exception.ExceptionNode.OuterXml + Environment.NewLine;
            }

            var newDocComment = XmlDocCommentHelper.CreateDocComment(comment, this.DocCommentNode.GetProject());
            SharedImplUtil.SetDocCommentBlockNode(this.MethodDeclarationModel.MethodDeclaration as ICSharpTypeMemberDeclarationNode, newDocComment);
        }
    }
}