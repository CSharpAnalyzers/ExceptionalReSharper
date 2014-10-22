using JetBrains.DocumentModel;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    internal abstract class ModelBase
    {
        public IAnalyzeUnit AnalyzeUnit { get; private set; }

        public abstract DocumentRange DocumentRange { get; }

        protected ModelBase(IAnalyzeUnit analyzeUnit)
        {
            AnalyzeUnit = analyzeUnit;
        }

        public virtual void Accept(AnalyzerBase analyzerBase)
        {
        }
    }
}