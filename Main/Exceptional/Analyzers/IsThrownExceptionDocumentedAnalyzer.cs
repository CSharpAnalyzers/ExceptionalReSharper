// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using CodeGears.ReSharper.Exceptional.Highlightings;
using CodeGears.ReSharper.Exceptional.Model;

using JetBrains.ReSharper.Daemon;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks that exceptions thrown outside are documented.</summary>
    internal class IsThrownExceptionDocumentedAnalyzer : AnalyzerBase
    {
        public IsThrownExceptionDocumentedAnalyzer(ExceptionalDaemonStageProcess process) 
            : base(process) { }

        public override void Visit(ThrownExceptionModel thrownExceptionModel)
        {
            if (thrownExceptionModel == null) return;
            if (thrownExceptionModel.AnalyzeUnit.IsPublicOrInternal == false) return;
            if (thrownExceptionModel.IsCatched) return;
            if (thrownExceptionModel.IsDocumented) return;

            this.Process.Hightlightings.Add(new HighlightingInfo(thrownExceptionModel.DocumentRange, new ExceptionNotDocumentedHighlighting(thrownExceptionModel), null, null));            
        }
    }
}