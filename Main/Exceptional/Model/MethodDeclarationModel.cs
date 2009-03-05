/// <copyright file="MethodDeclarationModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    /// <summary>Stores data about processed <see cref="IMethodDeclaration"/></summary>
    public class MethodDeclarationModel : ModelBase
    {
        public IMethodDeclaration MethodDeclaration { get; set; }
        public DocCommentBlockModel DocCommentBlockModel { get; private set; }
        public List<TryStatementModel> TryStatementModels { get; private set; }
        public List<CatchClauseModel> CatchClauseModels { get; private set; }
        public List<ThrowStatementModel> ThrowStatementModels { get; private set; }

        public MethodDeclarationModel(IMethodDeclaration methodDeclaration)
        {
            MethodDeclaration = methodDeclaration;
            TryStatementModels = new List<TryStatementModel>();
            CatchClauseModels = new List<CatchClauseModel>();
            ThrowStatementModels = new List<ThrowStatementModel>();
        }

        public void Initialize()
        {
            InitializeDocCommentBlockModel();
        }

        private void InitializeDocCommentBlockModel()
        {
            var docCommentBlockOwner = this.MethodDeclaration as IDocCommentBlockOwnerNode;
            if (docCommentBlockOwner == null) return;

            var docCommentBlockNode = docCommentBlockOwner.GetDocCommentBlockNode();
            if (docCommentBlockNode == null) return;
            
            this.DocCommentBlockModel = new DocCommentBlockModel(docCommentBlockNode);
        }

        public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if(this.DocCommentBlockModel != null)
                this.DocCommentBlockModel.AssignHighlights(process);

            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                catchClauseModel.AssignHighlights(process);
            }

            foreach (var throwStatementModel in this.ThrowStatementModels)
            {
                throwStatementModel.AssignHighlights(process);
            }

            foreach (var tryStatementModel in this.TryStatementModels)
            {
                tryStatementModel.AssignHighlights(process);
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            if (this.DocCommentBlockModel != null)
                this.DocCommentBlockModel.Accept(analyzerBase);

            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                catchClauseModel.Accept(analyzerBase);
            }

            foreach (var throwStatementModel in this.ThrowStatementModels)
            {
                throwStatementModel.Accept(analyzerBase);
            }

            foreach (var tryStatementModel in this.TryStatementModels)
            {
                tryStatementModel.Accept(analyzerBase);
            }
        }
    }
}