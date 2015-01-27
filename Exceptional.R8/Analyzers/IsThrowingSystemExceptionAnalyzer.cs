using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes whether a throw statement throws System.Exception.</summary>
    internal class IsThrowingSystemExceptionAnalyzer : AnalyzerBase
    {
        /// <summary>Performs analyze of <paramref name="thrownException"/>.</summary>
        /// <param name="thrownException">Thrown exception to analyze.</param>
        public override void Visit(ThrownExceptionModel thrownException)
        {
            if (thrownException.IsThrownFromThrowStatement && thrownException.ExceptionType.GetClrName().FullName == "System.Exception")
                ServiceLocator.StageProcess.Hightlightings.Add(new HighlightingInfo(thrownException.DocumentRange, new ThrowingSystemExceptionHighlighting(), null));
        }
    }
}