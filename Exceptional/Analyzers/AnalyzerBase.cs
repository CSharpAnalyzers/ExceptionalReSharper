using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>A base class for all analyzers.</summary>
    internal abstract class AnalyzerBase
    {
        /// <summary>Initializes a new instance of the <see cref="AnalyzerBase"/> class. </summary>
        /// <param name="process">The process. </param>
        /// <param name="settings">The settings. </param>
        protected AnalyzerBase(ExceptionalDaemonStageProcess process, ExceptionalSettings settings)
        {
            Process = process;
            Settings = settings;
        }

        /// <summary>Gets the current process. </summary>
        protected ExceptionalDaemonStageProcess Process { get; private set; }

        /// <summary>Gets the current settings. </summary>
        protected ExceptionalSettings Settings { get; private set; }

        /// <summary>Performs analyze of throw <paramref name="throwStatementModel"/>.</summary>
        /// <param name="throwStatementModel">Throw statement model to analyze.</param>
        public virtual void Visit(ThrowStatementModel throwStatementModel) { }

        /// <summary>Performs analyze of <paramref name="catchClauseModel"/>.</summary>
        /// <param name="catchClauseModel">Catch clause to analyze.</param>
        public virtual void Visit(CatchClauseModel catchClauseModel) { }

        /// <summary>Performs analyze of <paramref name="exceptionDocumentationModel"/>.</summary>
        /// <param name="exceptionDocumentationModel">Exception documentation to analyze.</param>
        public virtual void Visit(ExceptionDocCommentModel exceptionDocumentationModel) { }

        /// <summary>Performs analyze of <paramref name="thrownExceptionModel"/>.</summary>
        /// <param name="thrownExceptionModel">Thrown exception to analyze.</param>
        public virtual void Visit(ThrownExceptionModel thrownExceptionModel) { }
    }
}