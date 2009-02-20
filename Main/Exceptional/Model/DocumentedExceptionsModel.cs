using System.Collections.Generic;
using System.Xml;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    ///<summary>Represents exceptions documented in method comment.</summary>
    public class DocumentedExceptionsModel : IModel
    {
        public List<ExceptionDocCommentModel> DocumentedExceptions { get; private set; }

        private DocumentedExceptionsModel()
        {
            this.DocumentedExceptions = new List<ExceptionDocCommentModel>();
        }

        public static DocumentedExceptionsModel Create(IMethodDeclaration methodDeclaration)
        {
            var xmlNode = methodDeclaration.GetXMLDoc(false);
            if (xmlNode == null) return null;

            var exceptionNodes = xmlNode.SelectNodes("exception");
            if (exceptionNodes == null || exceptionNodes.Count == 0) return null;

            var result = new DocumentedExceptionsModel();

            foreach (XmlNode exceptionNode in exceptionNodes)
            {
                var model = ExceptionDocCommentModel.Create(exceptionNode, methodDeclaration);
                if (model.IsValid == false) continue;

                result.DocumentedExceptions.Add(model);
            }

            return result;
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            
        }

        public void Accept(Visitor visitor)
        {
            foreach (var exceptionDocCommentModel in this.DocumentedExceptions)
            {
                exceptionDocCommentModel.Accept(visitor);
            }
        }
    }
}