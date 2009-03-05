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
    public class ProcessContext
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

        public MethodDeclarationModel MethodDeclarationModel { get; set; }
        public Stack<TryStatementModel> TryStatementModelsStack { get; set; }
        public Stack<CatchClauseModel> CatchClauseModelsStack { get; set; }

        private ProcessContext()
        {
            this.TryStatementModelsStack = new Stack<TryStatementModel>();
            this.CatchClauseModelsStack = new Stack<CatchClauseModel>();
        }

        public void EnterTryBlock(ITryStatement tryStatement)
        {
            AssertMethodDeclaration();
            if(this.MethodDeclarationModel == null) return;

            var model = new TryStatementModel(tryStatement);
            this.MethodDeclarationModel.TryStatementModels.Add(model);
            this.TryStatementModelsStack.Push(model);
        }

        public void EndProcess(CSharpDaemonStageProcessBase process)
        {
            AssertMethodDeclaration();
            if (this.MethodDeclarationModel == null) return;

            var isThrownExceptionDocumentedExceptionAnalyzer = new IsThrownExceptionDocumentedExceptionAnalyzer();
            this.MethodDeclarationModel.Accept(isThrownExceptionDocumentedExceptionAnalyzer);

            var isDocumentedExceptionThrownAnalyzer = new IsDocumentedExceptionThrownAnalyzer();
            this.MethodDeclarationModel.Accept(isDocumentedExceptionThrownAnalyzer);

            this.MethodDeclarationModel.AssignHighlights(process);
            instance = null;
        }

        public void LeaveTryBlock()
        {
            this.TryStatementModelsStack.Pop();
        }

        public void EnterCatchClause(ICatchClause catchClause)
        {
            AssertMethodDeclaration();
            if (this.MethodDeclarationModel == null) return;

            //TODO: Do not create this model. Retrieve if from try statement
            var model = new CatchClauseModel(catchClause);
            this.MethodDeclarationModel.CatchClauseModels.Add(model);
            this.CatchClauseModelsStack.Push(model);
        }

        public void LeaveCatchClause()
        {
            this.CatchClauseModelsStack.Pop();
        }

        public void Process(ThrowStatementModel throwStatementModel)
        {
            AssertMethodDeclaration();
            if (this.MethodDeclarationModel == null) return;

            if (throwStatementModel == null) return;

            var catchedVisitor = new IsThrownExceptionCatchedAnalyzer();
            throwStatementModel.Accept(catchedVisitor);

            var visitor = new HasInnerExceptionFromOuterCatchClauseAnalyzer();
            throwStatementModel.Accept(visitor);

            this.MethodDeclarationModel.ThrowStatementModels.Add(throwStatementModel);
        }

        public void Process(CatchVariableModel catchVariableModel)
        {
            AssertMethodDeclaration();
            if (this.MethodDeclarationModel == null) return;

            Logger.Assert(this.CatchClauseModelsStack.Count > 0, "[Exceptional] There is no catch clause for catch variable declaration.");

            var catchClause = this.CatchClauseModelsStack.Peek();
            catchClause.VariableModel = catchVariableModel;
        }

        public void AssertMethodDeclaration()
        {
            Logger.Assert(this.MethodDeclarationModel != null, "[Exceptional] Method declaration cannot be null.");
        }
    }
}