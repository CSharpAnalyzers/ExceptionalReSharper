using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    /// <summary>A base class for all analyzers.</summary>
    public abstract class AnalyzerBase
    {
        /// <summary>Perform analyze of throw <paramref name="throwStatementModel"/>.</summary>
        /// <param name="throwStatementModel">A throw statement model to analyze.</param>
        public virtual void Visit(ThrowStatementModel throwStatementModel) {}

        /// <summary>Perform analyze of <paramref name="catchAllClauseModel"/>.</summary>
        /// <param name="catchAllClauseModel">A catch-all clause to analyze.</param>
        public virtual void Visit(CatchClauseModel catchAllClauseModel) { }

        /// <summary>Perform analyze of <paramref name="exceptionDocumentationModel"/>.</summary>
        /// <param name="exceptionDocumentationModel">Exception documentation to analyze.</param>
        public virtual void Visit(ExceptionDocumentationModel exceptionDocumentationModel) { }

        /// <summary>Perform analyze of <paramref name="docCommentBlockModel"/>.</summary>
        /// <param name="docCommentBlockModel">Documentation to analyze.</param>
        public virtual void Visit(DocCommentBlockModel docCommentBlockModel) { }
    }
}