using JetBrains.DocumentModel;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    internal abstract class ModelBase
    {
        /// <summary>Gets the analyze unit. </summary>
        public IAnalyzeUnit AnalyzeUnit { get; private set; }

        /// <summary>Gets the document range of this object. </summary>
        public abstract DocumentRange DocumentRange { get; }

        protected ModelBase(IAnalyzeUnit analyzeUnit)
        {
            AnalyzeUnit = analyzeUnit;
        }

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public virtual void Accept(AnalyzerBase analyzer)
        {
        }
    }
}