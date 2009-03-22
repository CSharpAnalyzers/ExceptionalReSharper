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
        void EnterTryBlock(ITryStatementNode tryStatement);
        void LeaveTryBlock();
        void EnterCatchClause(ICatchClauseNode catchClause);
        void LeaveCatchClause();
        void Process(IThrowStatementNode throwStatement);
        void Process(ICatchVariableDeclarationNode catchVariableDeclaration);
        void Process(IInvocationExpressionNode invocationExpression);
        void Process(IDocCommentBlockNode docCommentBlockNode);
        void EnterAccessor(IAccessorDeclarationNode accessorDeclarationNode);
        void LeaveAccessor();
    }
}