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
    internal abstract class ProcessContext<T> : IProcessContext where T : IAnalyzeUnit
    {
        private static readonly List<AnalyzerBase> _analyzers = new List<AnalyzerBase>(
            new AnalyzerBase[]
                {
                    new IsThrownExceptionDocumentedAnalyzer()
                    , new IsDocumentedExceptionThrownAnalyzer()
                    , new CatchAllClauseAnalyzer()
                    , new HasInnerExceptionFromOuterCatchClauseAnalyzer()
                });

        protected IAnalyzeUnit AnalyzeUnit { get; private set; }
        protected T Model { get; private set; }
        private Stack<TryStatementModel> TryStatementModelsStack { get; set; }
        private Stack<CatchClauseModel> CatchClauseModelsStack { get; set; }
        protected Stack<IBlockModel> BlockModelsStack { get; set; }

        protected ProcessContext()
        {
            this.TryStatementModelsStack = new Stack<TryStatementModel>();
            this.CatchClauseModelsStack = new Stack<CatchClauseModel>();
            this.BlockModelsStack = new Stack<IBlockModel>();
        }

        public void StartProcess(IAnalyzeUnit analyzeUnit)
        {
            this.AnalyzeUnit = analyzeUnit;
            this.Model = (T)analyzeUnit;
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

        public void EnterCatchClause(ICatchClauseNode catchClauseNode)
        {
            if (this.IsValid() == false) return;
            if (catchClauseNode == null) return;
            Logger.Assert(this.TryStatementModelsStack.Count > 0, "[Exceptional] There is no try statement for catch declaration.");

            var tryStatementModel = this.TryStatementModelsStack.Peek();
            var model = tryStatementModel.CatchClauseModels.Find(catchClauseModel => catchClauseModel.Node.Equals(catchClauseNode));

            //var model = new CatchClauseModel(this.AnalyzeUnit, catchClauseNode);
            //model.ParentBlock = tryStatementModel.ParentBlock;
            //tryStatementModel.CatchClauseModels.Add(model);
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

        public virtual void EnterAccessor(IAccessorDeclarationNode accessorDeclarationNode) {}
        public virtual void LeaveAccessor() {}
    }
}