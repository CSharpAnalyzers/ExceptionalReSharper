using CodeGears.ReSharper.Exceptional.Model;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    internal class NullProcessContext : IProcessContext
    {
        public void StartProcess(IAnalyzeUnit analyzeUnit)
        {
        }

        public void EndProcess(CSharpDaemonStageProcessBase process)
        {
        }

        public void EnterTryBlock(ITryStatementNode tryStatement)
        {
        }

        public void LeaveTryBlock()
        {
        }

        public void EnterCatchClause(ICatchClauseNode catchClauseNode)
        {
        }

        public void LeaveCatchClause()
        {
        }

        public void Process(IThrowStatementNode throwStatement)
        {
        }

        public void Process(ICatchVariableDeclarationNode catchVariableDeclaration)
        {
        }

        public void Process(IReferenceExpressionNode invocationExpression)
        {
        }

        public void Process(IDocCommentBlockNode docCommentBlockNode)
        {
        }

        public void EnterAccessor(IAccessorDeclarationNode accessorDeclarationNode)
        {
        }

        public void LeaveAccessor()
        {
        }
    }
}