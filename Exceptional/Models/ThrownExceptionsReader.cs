using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.Logging;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Extracts thrown exceptions. </summary>
    internal static class ThrownExceptionsReader
    {
        /// <summary>Reads the specified reference expression. </summary>
        /// <param name="referenceExpression">The reference expression.</param>
        /// <returns>The list of thrown exceptions. </returns>
        public static IEnumerable<ThrownExceptionDocumentationModel> Read(IReferenceExpression referenceExpression)
        {
            var result = new List<ThrownExceptionDocumentationModel>();

            var resolveResult = referenceExpression.Reference.Resolve();
            var declaredElement = resolveResult.DeclaredElement;
            if (declaredElement == null)
                return result;

            var declarations = declaredElement.GetDeclarations();
            if (declarations.Count == 0)
                return GetFromXmlDoc(declaredElement, referenceExpression.GetPsiModule());

            foreach (var declaration in declarations)
            {
                var docCommentBlockOwnerNode = declaration as IDocCommentBlockOwnerNode;
                if (docCommentBlockOwnerNode == null) return result;

                var docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
                if (docCommentBlockNode == null) return result;

                var docCommentBlockModel = new DocCommentBlockModel(null, docCommentBlockNode);

                foreach (var exceptionDocCommentModel in docCommentBlockModel.ExceptionDocCommentModels)
                {
                    var model = new ThrownExceptionDocumentationModel(
                        exceptionDocCommentModel.ExceptionType, exceptionDocCommentModel.ExceptionDescription);
                    result.Add(model);
                }
            }

            return result;
        }

        private static IEnumerable<ThrownExceptionDocumentationModel> GetFromXmlDoc(IDeclaredElement declaredElement, IPsiModule psiModule)
        {
            // TODO: This is the code where the XML doc from an external (eg system assembly) is read, 
            // this sometimes fail if the xml doc is missing!? => is there a solution for this?

            var result = new List<ThrownExceptionDocumentationModel>();

            var xmlNode = declaredElement.GetXMLDoc(true);
            if (xmlNode == null)
                return result;

            var exceptionNodes = xmlNode.SelectNodes("exception");
            if (exceptionNodes == null)
                return result;

            foreach (XmlNode exceptionNode in exceptionNodes)
            {
                if (exceptionNode.Attributes != null)
                {
                    var exceptionType = exceptionNode.Attributes["cref"].Value;

                    if (exceptionType.StartsWith("T:"))
                        exceptionType = exceptionType.Substring(2);

                    var exceptionDeclaredType = TypeFactory.CreateTypeByCLRName(exceptionType, psiModule,
                        psiModule.GetContextFromModule());

                    Logger.Assert(exceptionDeclaredType != null, "Created exception type was null!");

                    result.Add(new ThrownExceptionDocumentationModel(exceptionDeclaredType, exceptionNode.Value));
                }
            }

            return result;
        }
    }
}