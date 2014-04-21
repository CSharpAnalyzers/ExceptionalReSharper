// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.Logging;

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
            if (stageProcess == null) yield break;

            yield return new IsThrownExceptionDocumentedAnalyzer(stageProcess);
            yield return new IsDocumentedExceptionThrownAnalyzer(stageProcess);
            yield return new CatchAllClauseAnalyzer(stageProcess);
            yield return new HasInnerExceptionFromOuterCatchClauseAnalyzer(stageProcess);
        }

        protected ProcessContext()
        {
            TryStatementModelsStack = new Stack<TryStatementModel>();
            CatchClauseModelsStack = new Stack<CatchClauseModel>();
            BlockModelsStack = new Stack<IBlockModel>();
        }

        public void StartProcess(IAnalyzeUnit analyzeUnit)
        {
            AnalyzeUnit = analyzeUnit;
            Model = (T)analyzeUnit;
            BlockModelsStack.Push(AnalyzeUnit);
        }

        public void EndProcess(CSharpDaemonStageProcessBase process)
        {
            if (IsValid() == false) return;

            foreach (var analyzerBase in ProvideAnalyzers(process as ExceptionalDaemonStageProcess))
            {
                AnalyzeUnit.Accept(analyzerBase);
            }
        }

        public void EnterTryBlock(ITryStatement tryStatement)
        {
            if (IsValid() == false) return;
            if (tryStatement == null) return;

            Logger.Assert(BlockModelsStack.Count > 0, "[Exceptional] There is no block for try statement.");

            var model = new TryStatementModel(AnalyzeUnit, tryStatement);

            var blockModel = BlockModelsStack.Peek();
            blockModel.TryStatementModels.Add(model);
            model.ParentBlock = blockModel;

            TryStatementModelsStack.Push(model);
            BlockModelsStack.Push(model);
        }

        public void LeaveTryBlock()
        {
            TryStatementModelsStack.Pop();
            BlockModelsStack.Pop();
        }

        public void EnterCatchClause(ICatchClause catchClauseNode)
        {
            if (IsValid() == false) return;
            if (catchClauseNode == null) return;

            Logger.Assert(TryStatementModelsStack.Count > 0,
                          "[Exceptional] There is no try statement for catch declaration.");

            var tryStatementModel = TryStatementModelsStack.Peek();
            var model =
                tryStatementModel.CatchClauseModels.Find(
                    catchClauseModel => catchClauseModel.Node.Equals(catchClauseNode));

            Logger.Assert(model != null, "[Exceptional] Cannot find catch model!");

            CatchClauseModelsStack.Push(model);
            BlockModelsStack.Push(model);
        }

        public void LeaveCatchClause()
        {
            CatchClauseModelsStack.Pop();
            BlockModelsStack.Pop();
        }

        public void Process(IThrowStatement throwStatement)
        {
            if (IsValid() == false) return;
            if (throwStatement == null) return;

            Logger.Assert(BlockModelsStack.Count > 0, "[Exceptional] There is no block for throw statement.");

            new ThrowStatementModel(AnalyzeUnit, throwStatement, BlockModelsStack.Peek());
        }

        public void Process(ICatchVariableDeclaration catchVariableDeclaration)
        {
            if (IsValid() == false) return;
            if (catchVariableDeclaration == null) return;

            Logger.Assert(CatchClauseModelsStack.Count > 0,
                          "[Exceptional] There is no catch clause for catch variable declaration.");

            var catchClause = CatchClauseModelsStack.Peek();
            catchClause.VariableModel = new CatchVariableModel(AnalyzeUnit, catchVariableDeclaration);
        }

        public void Process(IReferenceExpression invocationExpression)
        {
            if (IsValid() == false) return;
            if (invocationExpression == null) return;

            Logger.Assert(BlockModelsStack.Count > 0, "[Exceptional] There is no block for invocation statement.");

            new ReferenceExpressionModel(AnalyzeUnit, invocationExpression, BlockModelsStack.Peek());
        }

        protected bool IsValid()
        {
            return AnalyzeUnit != null;
        }

        public void Process(IDocCommentBlockNode docCommentBlockNode)
        {
            if (IsValid() == false) return;

            AnalyzeUnit.DocCommentBlockModel = new DocCommentBlockModel(AnalyzeUnit, docCommentBlockNode);
        }

        public virtual void EnterAccessor(IAccessorDeclaration accessorDeclarationNode)
        {
        }

        public virtual void LeaveAccessor()
        {
        }
    }
}