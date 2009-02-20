using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public interface IModel
    {
        void AssignHighlights(CSharpDaemonStageProcessBase process);
        void Accept(Visitor visitor);
    }
}