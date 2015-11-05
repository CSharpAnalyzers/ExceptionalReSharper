using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.Logging;
using ReSharper.Exceptional.Models.ExceptionsOrigins;

namespace ReSharper.Exceptional.Models
{
    /// <summary>Extracts thrown exceptions. </summary>
    internal static class ThrownExceptionsReader
    {
        /// <summary>Reads the specified reference expression. </summary>
        /// <param name="analyzeUnit">The analyze unit. </param>
        /// <param name="exceptionsOrigin">The exceptions origin. </param>
        /// <param name="referenceExpression">The reference expression.</param>
        /// <returns>The list of thrown exceptions. </returns>
        public static IEnumerable<ThrownExceptionModel> Read(IAnalyzeUnit analyzeUnit, IExceptionsOriginModel exceptionsOrigin, IReferenceExpression referenceExpression)
        {
            var result = new List<ThrownExceptionModel>();

            var resolveResult = referenceExpression.Parent is IElementAccessExpression ? 
                ((IElementAccessExpression)referenceExpression.Parent).Reference.Resolve() : 
                referenceExpression.Reference.Resolve();

            var declaredElement = resolveResult.DeclaredElement;
            if (declaredElement == null)
                return result;

            var declarations = declaredElement.GetDeclarations();
            if (declarations.Count == 0)
                return Read(analyzeUnit, exceptionsOrigin, declaredElement);

            foreach (var declaration in declarations)
            {
#if R8
                var docCommentBlockOwnerNode = declaration as IDocCommentBlockOwnerNode;
                if (docCommentBlockOwnerNode == null)
                    return result;

                var docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
                if (docCommentBlockNode == null)
                    return result;
#endif
#if R9 || R10
                var docCommentBlockOwnerNode = declaration as IDocCommentBlockOwner;
                if (docCommentBlockOwnerNode == null)
                    return result;

                var docCommentBlockNode = docCommentBlockOwnerNode.DocCommentBlock;
                if (docCommentBlockNode == null)
                    return result;
#endif

                var docCommentBlockModel = new DocCommentBlockModel(null, docCommentBlockNode);
                foreach (var comment in docCommentBlockModel.DocumentedExceptions)
                {
                    result.Add(new ThrownExceptionModel(analyzeUnit, exceptionsOrigin, comment.ExceptionType,
                        comment.ExceptionDescription, false, comment.Accessor));
                }
            }

            return result;
        }

        public static IEnumerable<ThrownExceptionModel> Read(IAnalyzeUnit analyzeUnit, IExceptionsOriginModel exceptionsOrigin, IDeclaredElement declaredElement)
        {
            if (declaredElement == null)
                return new List<ThrownExceptionModel>();

            var xmlDoc = declaredElement.GetXMLDoc(true);
            if (xmlDoc == null)
                return new List<ThrownExceptionModel>();

            return Read(analyzeUnit, exceptionsOrigin, xmlDoc);
        }

        public static IEnumerable<ThrownExceptionModel> Read(IAnalyzeUnit analyzeUnit, ObjectCreationExpressionModel objectCreationExpression)
        {
            var declaredElement = objectCreationExpression.Node.ConstructorReference.Resolve().DeclaredElement;
            if (declaredElement == null)
                return Enumerable.Empty<ThrownExceptionModel>();

            var xmlDoc = declaredElement.GetXMLDoc(true);
            if (xmlDoc == null)
                return Enumerable.Empty<ThrownExceptionModel>();

            return Read(analyzeUnit, objectCreationExpression, xmlDoc);
        }

        private static IEnumerable<ThrownExceptionModel> Read(IAnalyzeUnit analyzeUnit, IExceptionsOriginModel exceptionsOrigin, XmlNode xmlDoc)
        {
            var result = new List<ThrownExceptionModel>();

            var exceptionNodes = xmlDoc.SelectNodes("exception");
            if (exceptionNodes == null)
                return result;

            var psiModule = analyzeUnit.GetPsiModule();
            foreach (XmlNode exceptionNode in exceptionNodes)
            {
                if (exceptionNode.Attributes != null)
                {
                    var accessorNode = exceptionNode.Attributes["accessor"];
                    var accessor = accessorNode != null ? accessorNode.Value : null;

                    var exceptionType = exceptionNode.Attributes["cref"].Value;
                    if (exceptionType.StartsWith("T:"))
                        exceptionType = exceptionType.Substring(2);

#if R10
                    var exceptionDeclaredType = TypeFactory.CreateTypeByCLRName(exceptionType, psiModule);
#else
                    var exceptionDeclaredType = TypeFactory.CreateTypeByCLRName(exceptionType, psiModule, psiModule.GetContextFromModule());
#endif

                    result.Add(new ThrownExceptionModel(analyzeUnit, exceptionsOrigin, exceptionDeclaredType, 
                        exceptionNode.InnerXml, false, accessor));
                }
            }

            return result;
        }
    }
}