using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Contexts
{
    internal interface IProcessContext
    {
        IAnalyzeUnit Model { get; }

        void StartProcess(IAnalyzeUnit analyzeUnit);
        void RunAnalyzers();

        void EnterTryBlock(ITryStatement tryStatement);
        void LeaveTryBlock();

        void EnterCatchClause(ICatchClause catchClauseNode);
        void LeaveCatchClause();

        void Process(IThrowStatement throwStatement);
        void Process(ICatchVariableDeclaration catchVariableDeclaration);
        void Process(IReferenceExpression invocationExpression);
        void Process(IObjectCreationExpression objectCreationExpression);
#if R8
        void Process(IDocCommentBlockNode docCommentBlockNode);
#endif
#if R9 || R10
        void Process(IDocCommentBlock docCommentBlockNode);
#endif

        void EnterAccessor(IAccessorDeclaration accessorDeclarationNode);
        void LeaveAccessor();
    }
}