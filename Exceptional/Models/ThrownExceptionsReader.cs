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
        /// <param name="analyzeUnit"></param>
        /// <param name="exceptionsOrigin"></param>
        /// <param name="referenceExpression">The reference expression.</param>
        /// <returns>The list of thrown exceptions. </returns>
        public static IEnumerable<ThrownExceptionModel> Read(IAnalyzeUnit analyzeUnit, IExceptionsOriginModel exceptionsOrigin, IReferenceExpression referenceExpression)
        {
            var result = new List<ThrownExceptionModel>();

            var resolveResult = referenceExpression.Reference.Resolve();
            var declaredElement = resolveResult.DeclaredElement;
            if (declaredElement == null)
                return result;

            var declarations = declaredElement.GetDeclarations();
            if (declarations.Count == 0)
                return GetFromXmlDoc(analyzeUnit, exceptionsOrigin, declaredElement, referenceExpression.GetPsiModule());

            foreach (var declaration in declarations)
            {
                var docCommentBlockOwnerNode = declaration as IDocCommentBlockOwnerNode;
                if (docCommentBlockOwnerNode == null)
                    return result;

                var docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
                if (docCommentBlockNode == null)
                    return result;

                var docCommentBlockModel = new DocCommentBlockModel(null, docCommentBlockNode);
                foreach (var comment in docCommentBlockModel.DocumentedExceptions)
                    result.Add(new ThrownExceptionModel(analyzeUnit, exceptionsOrigin, comment.ExceptionType, comment.ExceptionDescription));
            }

            return result;
        }

        private static IEnumerable<ThrownExceptionModel> GetFromXmlDoc(IAnalyzeUnit analyzeUnit, IExceptionsOriginModel exceptionsOrigin, IDeclaredElement declaredElement, IPsiModule psiModule)
        {
            var result = new List<ThrownExceptionModel>();

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
                    result.Add(new ThrownExceptionModel(analyzeUnit, exceptionsOrigin, exceptionDeclaredType, exceptionNode.InnerText));
                }
            }

            return result;
        }
    }
}