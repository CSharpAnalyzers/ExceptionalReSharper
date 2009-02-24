using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class CatchClauseModel : IModel
    {
        public ICatchClause CatchClause { get; private set; }
        public bool IsCatchAll { get; private set; }
        public bool IsRethrown { get; set; }

        private CatchClauseModel(ICatchClause catchClause)
        {
            CatchClause = catchClause;
        }

        public static CatchClauseModel Create(ICatchClause catchClause)
        {
            var model = new CatchClauseModel(catchClause);

            model.IsCatchAll = catchClause.ExceptionType == null || catchClause.ExceptionType.GetCLRName().Equals("System.ExceptionType");

            return model;
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if (this.IsCatchAll)
            {
                var treeNode = this.CatchClause.ToTreeNode();
                process.AddHighlighting(treeNode.CatchKeyword.GetDocumentRange(), new CatchAllClauseHighlighting());
            }

            if(this.IsRethrown == false)
            {
                var treeNode = this.CatchClause.ToTreeNode();
                process.AddHighlighting(treeNode.CatchKeyword.GetDocumentRange(), new SwallowedExceptionsHighlighting());
            }
        }

        public void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}