/// <copyright file="MethodDeclarationModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IMethodDeclaration"/></summary>
    internal class MethodDeclarationModel : ModelBase, IBlockModel
    {
        public IMethodDeclaration MethodDeclaration { get; set; }
        public DocCommentBlockModel DocCommentBlockModel { get; private set; }
        public List<TryStatementModel> TryStatementModels { get; private set; }
        public List<ThrowStatementModel> ThrowStatementModels { get; private set; }
        public IBlockModel ParentBlock { get; set; }

        public bool IsPublicOrInternal
        {
            get
            {
                if(this.MethodDeclaration == null) return false;
                var rights = this.MethodDeclaration.GetAccessRights();
                return rights == AccessRights.PUBLIC ||
                       rights == AccessRights.INTERNAL ||
                       rights == AccessRights.PROTECTED;
            }
        }

        public bool CatchesException(IDeclaredType exception)
        {
            return false;
        }

        public IDeclaredType GetCatchedException()
        {
            return null;
        }

        public IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCatched
        {
            get
            {
                foreach (var throwStatementModel in this.ThrowStatementModels)
                {
                    foreach (var thrownExceptionModel in throwStatementModel.ThrownExceptions)
                    {
                        if (thrownExceptionModel.IsCatched == false)
                        {
                            yield return thrownExceptionModel;
                        }
                    }
                }

                for (var i = 0; i < this.TryStatementModels.Count; i++)
                {
                    IBlockModel tryStatementModel = this.TryStatementModels[i];
                    foreach (var model in tryStatementModel.ThrownExceptionModelsNotCatched)
                    {
                        yield return model;
                    }
                }
            }
        }

        public MethodDeclarationModel(IMethodDeclaration methodDeclaration) : base(null)
        {
            MethodDeclaration = methodDeclaration;
            TryStatementModels = new List<TryStatementModel>();
            ThrowStatementModels = new List<ThrowStatementModel>();
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var tryStatementModel in this.TryStatementModels)
            {
                tryStatementModel.Accept(analyzerBase);
            }
            
            foreach (var throwStatementModel in this.ThrowStatementModels)
            {
                throwStatementModel.Accept(analyzerBase);
            }

            if (this.DocCommentBlockModel != null)
            {
                this.DocCommentBlockModel.Accept(analyzerBase);
            }
        }

        private void AddDocComment()
        {
            var docCommentBlockNode = CSharpElementFactory.GetInstance(this.MethodDeclaration.GetPsiModule()).CreateDocComment("<summary></summary>");
            SharedImplUtil.SetDocCommentBlockNode(this.MethodDeclaration.ToTreeNode(), docCommentBlockNode);

            SetDocCommentBlockNode(docCommentBlockNode);
        }

        public void EnsureHasDocComment()
        {
            if (this.DocCommentBlockModel == null)
            {
                AddDocComment();
            }
        }

        public void SetDocCommentBlockNode(IDocCommentBlockNode docCommentBlockNode)
        {
            this.DocCommentBlockModel = new DocCommentBlockModel(this, docCommentBlockNode);
        }
    }
}