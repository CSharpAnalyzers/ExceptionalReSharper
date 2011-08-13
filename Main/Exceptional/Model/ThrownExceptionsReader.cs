// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Extracts thrown exceptions.</summary>
    internal static class ThrownExceptionsReader
    {
        public static IEnumerable<IDeclaredType> Read(IReferenceExpression referenceExpression)
        {
            var result = new List<IDeclaredType>();

            var resolveResult = referenceExpression.Reference.Resolve();
            var declaredElement = resolveResult.DeclaredElement;
            if (declaredElement == null) return result;

            var declarations = declaredElement.GetDeclarations();
            if (declarations.Count == 0)
            {
                return GetFromXmlDoc(declaredElement, referenceExpression.GetPsiModule());
            }

            foreach (var declaration in declarations)
            {
                var docCommentBlockOwnerNode = declaration as IDocCommentBlockOwnerNode;
                if (docCommentBlockOwnerNode == null) return result;

                var docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
                if (docCommentBlockNode == null) return result;

                var docCommentBlockModel = new DocCommentBlockModel(null, docCommentBlockNode);

                foreach (var exceptionDocCommentModel in docCommentBlockModel.ExceptionDocCommentModels)
                {
                    result.Add(exceptionDocCommentModel.ExceptionType);
                }
            }

            return result;
        }

        private static IEnumerable<IDeclaredType> GetFromXmlDoc(IDeclaredElement declaredElement, IPsiModule psiModule)
        {
            var result = new List<IDeclaredType>();

            var xmlNode = declaredElement.GetXMLDoc(true);
            if (xmlNode == null) return result;

            var exceptionNodes = xmlNode.SelectNodes("exception");
            if (exceptionNodes == null) return result;

            foreach (XmlNode exceptionNode in exceptionNodes)
            {
                var exceptionType = exceptionNode.Attributes["cref"].Value;

                if (exceptionType.StartsWith("T:"))
                {
                    exceptionType = exceptionType.Substring(2);
                }

                var exceptionDecaredType = TypeFactory.CreateTypeByCLRName(exceptionType, psiModule);

                Logger.Assert(exceptionDecaredType != null, "Created exception type was null!");
                result.Add(exceptionDecaredType);
            }

            return result;
        }
    }
}