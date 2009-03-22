using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public static class InvocationExceptionsReader
    {
        public static List<IDeclaredType> Read(IInvocationExpressionNode invocationExpression)
        {
            var result = new List<IDeclaredType>();

            var referenceExpression = invocationExpression.InvokedExpressionNode as IReferenceExpressionNode;
            if (referenceExpression == null) return result;

            var resolveResult = referenceExpression.Reference.Resolve();
            var declaredElement = resolveResult.DeclaredElement;
            if (declaredElement == null) return result;

            var declarations = declaredElement.GetDeclarations();
            if (declarations == null || declarations.Count == 0)
            {
                return GetFromXmlDoc(declaredElement, invocationExpression.GetPsiModule());
            }

            var docCommentBlockOwnerNode = declarations[0] as IDocCommentBlockOwnerNode;
            if (docCommentBlockOwnerNode == null) return result;

            var docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
            if (docCommentBlockNode == null) return result;

            var docCommentBlockModel = new DocCommentBlockModel(null, docCommentBlockNode);

            foreach (var exceptionDocCommentModel in docCommentBlockModel.ExceptionDocCommentModels)
            {
                result.Add(exceptionDocCommentModel.ExceptionType);
            }

            return result;
        }

        private static List<IDeclaredType> GetFromXmlDoc(IDeclaredElement declaredElement, IPsiModule psiModule)
        {
            var result = new List<IDeclaredType>();

            var xmlNode = declaredElement.GetXMLDoc(false);
            if (xmlNode == null) return result;


            var exceptionNodes = xmlNode.SelectNodes("exception");
            if (exceptionNodes == null) return result;

            foreach (XmlNode exceptionNode in exceptionNodes)
            {
                var exceptionType = exceptionNode.Attributes["cref"].Value;

                if (exceptionType.StartsWith("T:"))
                    exceptionType = exceptionType.Substring(2);

                var exceptionDecaredType = TypeFactory.CreateTypeByCLRName(exceptionType, psiModule);

                Logger.Assert(exceptionDecaredType != null, "Created exception type was null!");
                result.Add(exceptionDecaredType);
            }

            return result;
        }
    }
}