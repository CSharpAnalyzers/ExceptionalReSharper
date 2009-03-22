/// <copyright file="InvocationModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class InvocationModel : TreeElementModelBase<IInvocationExpressionNode>, IExceptionsOriginModel
    {
        public List<ThrownExceptionModel> ThrownExceptions { get; set; }
        public IBlockModel ContainingBlockModel { get; private set; }

        public InvocationModel(IAnalyzeUnit analyzeUnit, IInvocationExpressionNode invocationExpression, IBlockModel containingBlockModel)
            : base(analyzeUnit, invocationExpression)
        {
            ContainingBlockModel = containingBlockModel;
            containingBlockModel.ExceptionOriginModels.Add(this);

            ThrownExceptions = GetThrownExceptions();
        }

        private List<ThrownExceptionModel> GetThrownExceptions()
        {
            var result = new List<ThrownExceptionModel>();

            var referenceExpression = this.Node.InvokedExpressionNode as IReferenceExpressionNode;
            if (referenceExpression == null) return result;

            var resolveResult = referenceExpression.Reference.Resolve();
            var declaredElement = resolveResult.DeclaredElement;
            if(declaredElement == null) return result;

            var declarations = declaredElement.GetDeclarations();
            if(declarations == null || declarations.Count == 0) return result;

            var docCommentBlockOwnerNode = declarations[0] as IDocCommentBlockOwnerNode;
            if(docCommentBlockOwnerNode == null) return result;

            var docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
            if (docCommentBlockNode == null) return result;

            var docCommentBlockModel = new DocCommentBlockModel(null, docCommentBlockNode);

            foreach (var exceptionDocCommentModel in docCommentBlockModel.ExceptionDocCommentModels)
            {
                var thrownException = new ThrownExceptionModel(
                    this.AnalyzeUnit, exceptionDocCommentModel.ExceptionType, this);

                result.Add(thrownException);
            }

            return result;
        }

        public bool Throws(IDeclaredType exceptionType)
        {
            foreach (var thrownExceptionModel in this.ThrownExceptions)
            {
                if (thrownExceptionModel.Throws(exceptionType))
                    return true;
            }

            return false;
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var thrownExceptionModel in this.ThrownExceptions)
            {
                thrownExceptionModel.Accept(analyzerBase);
            }
        }
    }
}