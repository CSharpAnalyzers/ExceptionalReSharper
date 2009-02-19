using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class CatchAllClauseModel : IModel
    {
        private ICatchClause CatchClause { get; set; }

        private CatchAllClauseModel(ICatchClause catchClause)
        {
            CatchClause = catchClause;
        }

        public static CatchAllClauseModel Create(ICatchClause catchClause)
        {
            var model = new CatchAllClauseModel(catchClause);

            return model;
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            var treeNode = this.CatchClause.ToTreeNode();
            process.AddHighlighting(treeNode.CatchKeyword.GetDocumentRange(), new CatchAllClauseHighlighting());
        }
    }
}