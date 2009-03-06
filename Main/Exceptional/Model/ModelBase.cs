using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class ModelBase
    {
        public MethodDeclarationModel MethodDeclarationModel { get; private set; }

        protected ModelBase(MethodDeclarationModel methodDeclarationModel)
        {
            MethodDeclarationModel = methodDeclarationModel;
        }

        public virtual void AssignHighlights(CSharpDaemonStageProcessBase process) {}
        public virtual void Accept(AnalyzerBase analyzerBase) {}
    }
}