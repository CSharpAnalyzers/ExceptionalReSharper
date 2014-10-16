using System.Linq;
using JetBrains.ReSharper.Daemon;
using ReSharper.Exceptional.Highlightings;

using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes an exception documentation and checks if it is thrown from the documented element.</summary>
    internal class IsDocumentedExceptionThrownAnalyzer : AnalyzerBase
    {
        public IsDocumentedExceptionThrownAnalyzer(ExceptionalDaemonStageProcess process, ExceptionalSettings settings) 
            : base(process, settings) { }

        public override void Visit(ExceptionDocCommentModel exceptionDocumentationModel)
        {
            if (exceptionDocumentationModel == null) 
                return;
            if (exceptionDocumentationModel.AnalyzeUnit.IsInspected == false)
                return;
            if (AnalyzeIfExceptionThrown(exceptionDocumentationModel))
                return;

            Process.Hightlightings.Add(new HighlightingInfo(exceptionDocumentationModel.DocumentRange, new ExceptionNotThrownHighlighting(exceptionDocumentationModel), null, null));            
        }

        private static bool AnalyzeIfExceptionThrown(ExceptionDocCommentModel exceptionDocumentationModel)
        {
            return exceptionDocumentationModel
                .AnalyzeUnit
                .ThrownExceptionModelsNotCaught
                .Any(m => m.Throws(exceptionDocumentationModel.ExceptionType));
        }
    }
}