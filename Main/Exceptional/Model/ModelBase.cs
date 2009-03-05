using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class ModelBase
    {
        public MethodDeclarationModel Parent
        {
            get { return ProcessContext.Instance.MethodDeclarationModel; }
        }

        public virtual void AssignHighlights(CSharpDaemonStageProcessBase process) {}
        public virtual void Accept(AnalyzerBase analyzerBase) {}
    }
}