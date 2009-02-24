using CodeGears.ReSharper.Exceptional.Model;

namespace CodeGears.ReSharper.Exceptional.Analyzers
{
    public abstract class Visitor
    {
        public virtual void Visit(ThrowStatementModel throwStatementModel) {}
        public virtual void Visit(ExceptionDocCommentModel exceptionDocCommentModel) { }
        public virtual void Visit(CatchClauseModel catchAllClauseModel) { }
    }
}