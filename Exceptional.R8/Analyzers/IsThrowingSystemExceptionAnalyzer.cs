using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes whether a throw statement throws System.Exception.</summary>
    internal class IsThrowingSystemExceptionAnalyzer : AnalyzerBase
    {
        /// <summary>Initializes a new instance of the <see cref="CatchAllClauseAnalyzer"/> class. </summary>
        /// <param name="process">The process. </param>
        /// <param name="settings">The settings. </param>
        public IsThrowingSystemExceptionAnalyzer(ExceptionalDaemonStageProcess process, ExceptionalSettings settings) 
            : base(process, settings) { }

        /// <summary>Performs analyze of <paramref name="thrownException"/>.</summary>
        /// <param name="thrownException">Thrown exception to analyze.</param>
        public override void Visit(ThrownExceptionModel thrownException)
        {
            if (thrownException.IsThrownFromThrowStatement && thrownException.ExceptionType.GetClrName().FullName == "System.Exception")
                Process.Hightlightings.Add(new HighlightingInfo(thrownException.DocumentRange, new ThrowingSystemExceptionHighlighting(), null));
        }
    }
}