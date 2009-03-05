using CodeGears.ReSharper.Exceptional.Analyzers;
using CodeGears.ReSharper.Exceptional.Highlightings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class CatchClauseModel : ModelBase
    {
        public CatchVariableModel VariableModel { get; set; }

        public ICatchClause CatchClause { get; private set; }

        private ICatchClauseNode CatchClauseNode
        {
            get { return this.CatchClause as ICatchClauseNode; }
        }

        private IGeneralCatchClause GeneralCatchClause
        {
            get { return this.CatchClause as IGeneralCatchClause; }
        }

        private ISpecificCatchClause SpecificCatchClause
        {
            get { return this.CatchClause as ISpecificCatchClause; }
        }

        private ISpecificCatchClauseNode SpecificCatchClauseNode
        {
            get { return this.CatchClause as ISpecificCatchClauseNode; }
        }

        public bool IsCatchAll { get; private set; }
        public bool IsRethrown { get; set; }

        public bool HasVariable
        {
            get { return this.VariableModel != null; }
        }

        public CatchClauseModel(ICatchClause catchClause)
        {
            CatchClause = catchClause;

            this.IsCatchAll = catchClause.ExceptionType == null || catchClause.ExceptionType.GetCLRName().Equals("System.Exception");
        }

        public void AssignHighlights(CSharpDaemonStageProcessBase process)
        {
            if (this.IsCatchAll)
            {
                var treeNode = this.CatchClause.ToTreeNode();
                process.AddHighlighting(treeNode.CatchKeyword.GetDocumentRange(), new CatchAllClauseHighlighting(this));
            }

            if(this.IsRethrown == false)
            {
                var treeNode = this.CatchClause.ToTreeNode();
                process.AddHighlighting(treeNode.CatchKeyword.GetDocumentRange(), new SwallowedExceptionsHighlighting(this));
            }
        }

        public void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }

        public TextRange GetVariableTextRange()
        {
            return TextRange.InvalidRange;
        }

        public void AddVariable()
        {
            if (this.HasVariable) return;

            var codeFactory = new CodeElementFactory(this.CatchClause.GetProject());

            if (this.SpecificCatchClause != null)
            {
                var variableDeclaration =
                    codeFactory.CreateCatchVariableDeclarationNode(this.SpecificCatchClause.ExceptionType);

                this.SpecificCatchClauseNode.SetExceptionDeclarationNode(variableDeclaration);
                this.VariableModel = new CatchVariableModel(variableDeclaration);
            }
            else if(this.GeneralCatchClause != null)
            {
                var newCatch = codeFactory.CreateSpecificCatchClause(null, this.CatchClause.Body);
                if (newCatch == null) return;

                this.GeneralCatchClause.ReplaceBy(newCatch);

                this.CatchClause = newCatch;
                this.VariableModel = new CatchVariableModel(newCatch.ExceptionDeclaration);
            }
        }

        public bool Catches(IDeclaredType exception)
        {
            if(this.CatchClause.ExceptionType != null)
            {
                return this.CatchClause.ExceptionType.Equals(exception);
            }

            return exception.GetCLRName().Equals("System.Exception");
        }
    }
}