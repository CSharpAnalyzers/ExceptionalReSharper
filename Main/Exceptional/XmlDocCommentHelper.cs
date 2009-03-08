/// <copyright file="XmlDocCommentHelper.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional
{
    public class XmlDocCommentHelper
    {
        public static TextRange InsertExceptionDocumentation(ICSharpTypeMemberDeclarationNode memberDeclaration, string exceptionName)
        {
            var comment = SharedImplUtil.GetDocCommentBlockNode(memberDeclaration);
            var text = comment != null ? comment.GetText() + Environment.NewLine : String.Empty;

            text += String.Format("/// <exception cref=\"{0}\"></exception>", exceptionName) + Environment.NewLine +
                    " public void foo() {}";

            var commentOwner = CSharpElementFactory.GetInstance(memberDeclaration.GetProject()).CreateTypeMemberDeclaration(text) as IDocCommentBlockOwnerNode;
            var docCommentNode = commentOwner.GetDocCommentBlockNode();
            SharedImplUtil.SetDocCommentBlockNode(memberDeclaration, docCommentNode);

            return TextRange.InvalidRange;
        }

        public static void RemoveExceptionDocumentation(ICSharpTypeMemberDeclarationNode memberDeclaration, string documentationText)
        {
            var comment = SharedImplUtil.GetDocCommentBlockNode(memberDeclaration);
            var commentText = comment.GetText();

            var result = commentText.Replace(documentationText, String.Empty);

            result += Environment.NewLine + " public void foo() {}";

            var commentResult = CSharpElementFactory.GetInstance(memberDeclaration.GetProject()).CreateTypeMemberDeclaration(result) as IDocCommentBlockOwnerNode;

            SharedImplUtil.SetDocCommentBlockNode(memberDeclaration, commentResult.GetDocCommentBlockNode());
        }

        public static IDocCommentBlockNode CreateDocComment(string commentText, IProject project)
        {
            commentText += Environment.NewLine + " public void foo() {}";

            var commentResult = CSharpElementFactory.GetInstance(project).CreateTypeMemberDeclaration(commentText) as IDocCommentBlockOwnerNode;
            if(commentResult == null) return null;

            return commentResult.GetDocCommentBlockNode();
        }
    }
}