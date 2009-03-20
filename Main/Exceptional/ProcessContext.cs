/// <copyright file="ProcessContext.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
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
                    new IsThrownExceptionDocumentedAnalyzer()
                    , new IsDocumentedExceptionThrownAnalyzer()
                    , new CatchAllClauseAnalyzer()
                    , new HasInnerExceptionFromOuterCatchClauseAnalyzer()
                });

        private IAnalyzeUnit AnalyzeUnit { get; set; }
        private Stack<TryStatementModel> TryStatementModelsStack { get; set; }
        private Stack<CatchClauseModel> CatchClauseModelsStack { get; set; }
        private Stack<IBlockModel> BlockModelsStack { get; set; }

        private ProcessContext()
        {
            this.TryStatementModelsStack = new Stack<TryStatementModel>();
            this.CatchClauseModelsStack = new Stack<CatchClauseModel>();
            this.BlockModelsStack = new Stack<IBlockModel>();
        }

        public void StartProcess(IAnalyzeUnit analyzeUnit)
        {
            this.AnalyzeUnit = analyzeUnit;
            this.BlockModelsStack.Push(this.AnalyzeUnit);
        }

        public void EndProcess(CSharpDaemonStageProcessBase process)
        {
            if (this.IsValid() == false) return;

            foreach (var analyzerBase in _analyzers)
            {
                analyzerBase.Process = process;
                this.AnalyzeUnit.Accept(analyzerBase);
            }

            instance = null;
        }

        public void EnterTryBlock(ITryStatementNode tryStatement)
        {
            if (this.IsValid() == false) return;
            if (tryStatement == null) return;
            Logger.Assert(this.BlockModelsStack.Count > 0, "[Exceptional] There is no block for try statement.");

            var model = new TryStatementModel(this.AnalyzeUnit, tryStatement);

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

        public void EnterCatchClause(ICatchClauseNode catchClause)
        {
            if (this.IsValid() == false) return;
            if (catchClause == null) return;
            Logger.Assert(this.TryStatementModelsStack.Count > 0, "[Exceptional] There is no try statement for catch declaration.");

            var tryStatementModel = this.TryStatementModelsStack.Peek();

            var model = new CatchClauseModel(this.AnalyzeUnit, catchClause);
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

        public void Process(IThrowStatementNode throwStatement)
        {
            if (this.IsValid() == false) return;
            if (throwStatement == null) return;
            Logger.Assert(this.BlockModelsStack.Count > 0, "[Exceptional] There is no block for throw statement.");

            new ThrowStatementModel(this.AnalyzeUnit, throwStatement, this.BlockModelsStack.Peek());
        }

        public void Process(ICatchVariableDeclarationNode catchVariableDeclaration)
        {
            if (this.IsValid() == false) return;
            if (catchVariableDeclaration == null) return;

            Logger.Assert(this.CatchClauseModelsStack.Count > 0, "[Exceptional] There is no catch clause for catch variable declaration.");

            var catchClause = this.CatchClauseModelsStack.Peek();
            catchClause.VariableModel = new CatchVariableModel(this.AnalyzeUnit, catchVariableDeclaration);
        }

        public void Process(IInvocationExpressionNode invocationExpression)
        {
            if (this.IsValid() == false) return;
            if (invocationExpression == null) return;

            Logger.Assert(this.BlockModelsStack.Count > 0, "[Exceptional] There is no block for invocation statement.");

            new InvocationModel(this.AnalyzeUnit, invocationExpression, this.BlockModelsStack.Peek());
        }

        public bool IsValid()
        {
            return this.AnalyzeUnit != null;
        }

        public void Process(IDocCommentBlockNode docCommentBlockNode)
        {
            if (this.IsValid() == false) return;

            this.AnalyzeUnit.DocCommentBlockModel = new DocCommentBlockModel(this.AnalyzeUnit, docCommentBlockNode);
        }

        public void EnterAccessor(IAccessorDeclarationNode accessorDeclarationNode)
        {
            if (this.IsValid() == false) return;
            if (accessorDeclarationNode == null) return;
            Logger.Assert(this.AnalyzeUnit is PropertyDeclarationModel, "[Exceptional] We are not processing a property.");

            var parent = this.BlockModelsStack.Peek();

            var model = new AccessorDeclarationModel(this.AnalyzeUnit, accessorDeclarationNode);
            model.ParentBlock = parent;
            (this.AnalyzeUnit as PropertyDeclarationModel).Accessors.Add(model);
            this.BlockModelsStack.Push(model);
        }

        public void LeaveAccessor()
        {
            this.BlockModelsStack.Pop();
        }
    }
}