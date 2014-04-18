// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    internal interface IProcessContext
    {
        void StartProcess(IAnalyzeUnit analyzeUnit);
        void EndProcess(CSharpDaemonStageProcessBase process);
        void EnterTryBlock(ITryStatement tryStatement);
        void LeaveTryBlock();
        void EnterCatchClause(ICatchClause catchClauseNode);
        void LeaveCatchClause();
        void Process(IThrowStatement throwStatement);
        void Process(ICatchVariableDeclaration catchVariableDeclaration);
        void Process(IReferenceExpression invocationExpression);
        void Process(IDocCommentBlockNode docCommentBlockNode);
        void EnterAccessor(IAccessorDeclaration accessorDeclarationNode);
        void LeaveAccessor();
    }
}