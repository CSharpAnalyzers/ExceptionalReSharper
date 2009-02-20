using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class CatchAllClauseModel : IModel
    {
        private ICatchClause CatchClause { get; set; }

        private CatchAllClauseModel(ICatchClause catchClause)
        {
            CatchClause = catchClause;
        }

        public static CatchAllClauseModel Create(ICatchClause catchClause)
        {
            if (catchClause.ExceptionType != null && catchClause.ExceptionType.GetCLRName().Equals("System.Exception") == false)
                return null;

            var model = new CatchAllClauseModel(catchClause);

            return model;
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            var treeNode = this.CatchClause.ToTreeNode();
            process.AddHighlighting(treeNode.CatchKeyword.GetDocumentRange(), new CatchAllClauseHighlighting());
        }

        public void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}