// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
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
        protected IAnalyzeUnit AnalyzeUnit { get; private set; }
        protected T Model { get; private set; }
        private Stack<TryStatementModel> TryStatementModelsStack { get; set; }
        private Stack<CatchClauseModel> CatchClauseModelsStack { get; set; }
        protected Stack<IBlockModel> BlockModelsStack { get; private set; }

        private static IEnumerable<AnalyzerBase> ProvideAnalyzers(ExceptionalDaemonStageProcess stageProcess)
        {
            if(stageProcess == null) yield break;

            yield return new IsThrownExceptionDocumentedAnalyzer(stageProcess);
            yield return new IsDocumentedExceptionThrownAnalyzer(stageProcess);
            yield return new CatchAllClauseAnalyzer(stageProcess);
            yield return new HasInnerExceptionFromOuterCatchClauseAnalyzer(stageProcess);
        }

        protected ProcessContext()
        {
            this.TryStatementModelsStack = new Stack<TryStatementModel>();
            this.CatchClauseModelsStack = new Stack<CatchClauseModel>();
            this.BlockModelsStack = new Stack<IBlockModel>();
        }

        public void StartProcess(IAnalyzeUnit analyzeUnit)
        {
            this.AnalyzeUnit = analyzeUnit;
            this.Model = (T) analyzeUnit;
            this.BlockModelsStack.Push(this.AnalyzeUnit);
        }

        public void EndProcess(CSharpDaemonStageProcessBase process)
        {
            if (this.IsValid() == false) return;

            foreach (var analyzerBase in ProvideAnalyzers(process as ExceptionalDaemonStageProcess))
            {
                this.AnalyzeUnit.Accept(analyzerBase);
            }
        }

        public void EnterTryBlock(ITryStatement tryStatement)
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

        public void EnterCatchClause(ICatchClause catchClauseNode)
        {
            if (this.IsValid() == false) return;
            if (catchClauseNode == null) return;

            Logger.Assert(this.TryStatementModelsStack.Count > 0,
                          "[Exceptional] There is no try statement for catch declaration.");

            var tryStatementModel = this.TryStatementModelsStack.Peek();
            var model =
                tryStatementModel.CatchClauseModels.Find(
                    catchClauseModel => catchClauseModel.Node.Equals(catchClauseNode));

            Logger.Assert(model != null, "[Exceptional] Cannot find catch model!");

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

            new ThrowStatementModel(this.AnalyzeUnit, throwStatement, this.BlockModelsStack.Peek());
        }

        public void Process(ICatchVariableDeclaration catchVariableDeclaration)
        {
            if (this.IsValid() == false) return;
            if (catchVariableDeclaration == null) return;

            Logger.Assert(this.CatchClauseModelsStack.Count > 0,
                          "[Exceptional] There is no catch clause for catch variable declaration.");

            var catchClause = this.CatchClauseModelsStack.Peek();
            catchClause.VariableModel = new CatchVariableModel(this.AnalyzeUnit, catchVariableDeclaration);
        }

        public void Process(IReferenceExpression invocationExpression)
        {
            if (this.IsValid() == false) return;
            if (invocationExpression == null) return;

            Logger.Assert(this.BlockModelsStack.Count > 0, "[Exceptional] There is no block for invocation statement.");

            new ReferenceExpressionModel(this.AnalyzeUnit, invocationExpression, this.BlockModelsStack.Peek());
        }

        protected bool IsValid()
        {
            return this.AnalyzeUnit != null;
        }

        public void Process(IDocCommentBlockNode docCommentBlockNode)
        {
            if (this.IsValid() == false) return;

            this.AnalyzeUnit.DocCommentBlockModel = new DocCommentBlockModel(this.AnalyzeUnit, docCommentBlockNode);
        }

        public virtual void EnterAccessor(IAccessorDeclaration accessorDeclarationNode)
        {
        }

        public virtual void LeaveAccessor()
        {
        }
    }
}