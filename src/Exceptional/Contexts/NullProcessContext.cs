using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Contexts
{
    internal class NullProcessContext : IProcessContext
    {
        public IAnalyzeUnit Model { get { return null; } }

        public void StartProcess(IAnalyzeUnit analyzeUnit)
        {
        }

        public void RunAnalyzers()
        {
        }

        public void EnterTryBlock(ITryStatement tryStatement)
        {
        }

        public void LeaveTryBlock()
        {
        }

        public void EnterCatchClause(ICatchClause catchClauseNode)
        {
        }

        public void LeaveCatchClause()
        {
        }

        public void Process(IThrowStatement throwStatement)
        {
        }

        public void Process(ICatchVariableDeclaration catchVariableDeclaration)
        {
        }

        public void Process(IReferenceExpression invocationExpression)
        {
        }

        public void Process(IObjectCreationExpression objectCreationExpression)
        {

        }

        public void Process(IDocCommentBlock docCommentBlockNode)
        {

        }

        public void Process(IThrowExpression throwExpression)
        {
            
        }

        public void EnterAccessor(IAccessorDeclaration accessorDeclarationNode)
        {
        }

        public void LeaveAccessor()
        {
        }
    }
}