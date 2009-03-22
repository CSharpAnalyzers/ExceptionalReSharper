/// <copyright>Copyright (c) 2009 CodeGears.net All rights reserved.</copyright>

using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;

namespace CodeGears.ReSharper.Exceptional.Model
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