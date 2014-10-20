using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    internal interface IAnalyzeUnit : IBlockModel
    {
        DocCommentBlockModel DocCommentBlock { get; set; }
        bool IsInspected { get; }
        ITreeNode Node { get; }
        IDocCommentBlockNode AddDocCommentNode(IDocCommentBlockNode docCommentBlockNode);
        IPsiModule GetPsiModule();
        void Accept(AnalyzerBase analyzerBase);
    }
}