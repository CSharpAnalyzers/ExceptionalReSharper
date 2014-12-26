using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Models.ExceptionsOrigins;
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

        /// <summary>Performs analyze of throw <paramref name="throwStatement"/>.</summary>
        /// <param name="throwStatement">Throw statement model to analyze.</param>
        public virtual void Visit(ThrowStatementModel throwStatement) { }

        /// <summary>Performs analyze of <paramref name="catchClause"/>.</summary>
        /// <param name="catchClause">Catch clause to analyze.</param>
        public virtual void Visit(CatchClauseModel catchClause) { }

        /// <summary>Performs analyze of <paramref name="exceptionDocumentation"/>.</summary>
        /// <param name="exceptionDocumentation">Exception documentation to analyze.</param>
        public virtual void Visit(ExceptionDocCommentModel exceptionDocumentation) { }

        /// <summary>Performs analyze of <paramref name="thrownException"/>.</summary>
        /// <param name="thrownException">Thrown exception to analyze.</param>
        public virtual void Visit(ThrownExceptionModel thrownException) { }
    }
}