/// <copyright file="ProcessContext.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional
{
    internal class ProcessContext
    {
        private static ProcessContext instance;
        public static ProcessContext Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ProcessContext();
                }

                return instance;
            }
        }

        private static readonly List<AnalyzerBase> _analyzers = new List<AnalyzerBase>(
            new AnalyzerBase[]
                {
                    new IsThrownExceptionCatchedAnalyzer(),
                    new IsThrownExceptionDocumentedExceptionAnalyzer(),
                    new HasInnerExceptionFromOuterCatchClauseAnalyzer(),
                });

        public MethodDeclarationModel MethodDeclarationModel { get; private set; }
        public Stack<TryStatementModel> TryStatementModelsStack { get; private set; }
        public Stack<CatchClauseModel> CatchClauseModelsStack { get; private set; }
        public Stack<IBlockModel> BlockModelsStack { get; private set; }

        private ProcessContext()
        {
            this.TryStatementModelsStack = new Stack<TryStatementModel>();
            this.CatchClauseModelsStack = new Stack<CatchClauseModel>();
            this.BlockModelsStack = new Stack<IBlockModel>();
        }

        public void StartProcess(IMethodDeclaration methodDeclaration)
        {
            this.MethodDeclarationModel = new MethodDeclarationModel(methodDeclaration);
            this.MethodDeclarationModel.Initialize();
            this.BlockModelsStack.Push(this.MethodDeclarationModel);
        }

        public void EndProcess(CSharpDaemonStageProcessBase process)
        {
            if (this.IsValid() == false) return;

            foreach (var analyzerBase in _analyzers)
            {
                this.MethodDeclarationModel.Accept(analyzerBase);
            }

            this.MethodDeclarationModel.AssignHighlights(process);

            instance = null;
        }

        public void EnterTryBlock(ITryStatement tryStatement)
        {
            if (this.IsValid() == false) return;
            if (tryStatement == null) return;
            Logger.Assert(this.BlockModelsStack.Count > 0, "[Exceptional] There is no block for try statement.");

            var model = new TryStatementModel(this.MethodDeclarationModel, tryStatement);

            var blockModel = this.BlockModelsStack.Peek();
            blockModel.TryStatementModels.Add(model);
            model.ParentBlock = blockModel;

            this.TryStatementModelsStack.Push(model);
            this.BlockModelsStack.Push(model);
        }

        public void LeaveTryBlock()
        {
            this.TryStatementModelsStack.Pop();
            this.BlockModelsStack.Pop();
        }

        public void EnterCatchClause(ICatchClause catchClause)
        {
            if (this.IsValid() == false) return;
            if (catchClause == null) return;
            Logger.Assert(this.TryStatementModelsStack.Count > 0, "[Exceptional] There is no try statement for catch declaration.");

            var tryStatementModel = this.TryStatementModelsStack.Peek();

            var model = (catchClause is ISpecificCatchClauseNode) ? (CatchClauseModel)new SpecificCatchClauseModel(this.MethodDeclarationModel, catchClause) : new GeneralCatchClauseModel(this.MethodDeclarationModel, catchClause);
            model.ParentBlock = tryStatementModel.ParentBlock;
            tryStatementModel.CatchClauseModels.Add(model);
            this.CatchClauseModelsStack.Push(model);
            this.BlockModelsStack.Push(model);
        }

        public void LeaveCatchClause()
        {
            this.CatchClauseModelsStack.Pop();
            this.BlockModelsStack.Pop();
        }

        public void Process(IThrowStatement throwStatement)
        {
            if (this.IsValid() == false) return;
            if (throwStatement == null) return;
            Logger.Assert(this.BlockModelsStack.Count > 0, "[Exceptional] There is no block for throw statement.");

            new ThrowStatementModel(this.MethodDeclarationModel, throwStatement, this.BlockModelsStack.Peek());
        }

        public void Process(ICatchVariableDeclaration catchVariableDeclaration)
        {
            if (this.IsValid() == false) return;
            if (catchVariableDeclaration == null) return;

            Logger.Assert(this.CatchClauseModelsStack.Count > 0, "[Exceptional] There is no catch clause for catch variable declaration.");

            var catchClause = this.CatchClauseModelsStack.Peek();
            catchClause.VariableModel = new CatchVariableModel(this.MethodDeclarationModel, catchVariableDeclaration);
        }

        public bool IsValid()
        {
            return this.MethodDeclarationModel != null;
        }

        public void Process(ICSharpCommentNode commentNode)
        {
            if (commentNode.CommentType != CommentType.DOC_COMMENT) return;
            if (commentNode.CommentText.Contains("<exception") == false) return;

            new ExceptionDocumentationModel(this.MethodDeclarationModel, commentNode);
        }
    }
}