using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    internal interface IAnalyzeUnit : IBlockModel
    {
        ITreeNode Node { get; }
        IPsiModule GetPsiModule();

        DocCommentBlockModel DocumentationBlock { get; set; }

        bool IsInspected { get; }

        void Accept(AnalyzerBase analyzerBase);
    }
}