using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class CatchClauseModel : ModelBase, IBlockModel
    {
        public CatchVariableModel VariableModel { get; set; }

        protected ICatchClause CatchClause { get; set; }

        protected ICatchClauseNode CatchClauseNode
        {
            get { return this.CatchClause as ICatchClauseNode; }
        }

        public List<ThrowStatementModel> ThrowStatementModels { get; private set; }
        public List<TryStatementModel> TryStatementModels { get; private set; }

        public IBlockModel ParentBlock { get; set; }

        public abstract IDeclaredType GetCatchedException();

        public IEnumerable<ThrowStatementModel> ThrowStatementModelsNotCatched
        {
            get
            {
                foreach (var throwStatementModel in this.ThrowStatementModels)
                {
                    if (throwStatementModel.IsCatched == false)
                    {
                        yield return throwStatementModel;
                    }
                }

                for (var i = 0; i < this.TryStatementModels.Count; i++)
                {
                    IBlockModel tryStatementModel = this.TryStatementModels[i];
                    foreach (var model in tryStatementModel.ThrowStatementModelsNotCatched)
                    {
                        yield return model;
                    }
                }
            }
        }

        public abstract void AddVariable();
        public abstract bool Catches(IDeclaredType exception);
        
        public bool IsCatchAll { get; private set; }
        public bool IsRethrown { get; set; }

        public bool HasVariable
        {
            get { return this.VariableModel != null; }
        }

        public override DocumentRange DocumentRange
        {
            get { return this.CatchClauseNode.GetDocumentRange(); }
        }

        public abstract bool HasExceptionType { get; }

        protected CatchClauseModel(MethodDeclarationModel methodDeclarationModel, ICatchClause catchClause)
            : base(methodDeclarationModel)
        {
            CatchClause = catchClause;
            ThrowStatementModels = new List<ThrowStatementModel>();
            TryStatementModels = new List<TryStatementModel>();

            this.IsCatchAll = GetIsCatchAll();
            this.IsRethrown = GetIsRethrown();
        }

        private bool GetIsRethrown()
        {
            //TODO: Implement
            return true;
        }

        private bool GetIsCatchAll()
        {
            //TODO: Implement
            return false;
        }

        public bool CatchesException(IDeclaredType exception)
        {
            return this.ParentBlock.CatchesException(exception);
        }

        //public override void AssignHighlights(CSharpDaemonStageProcessBase process)
        //{
        //    if(this.IsRethrown == false)
        //    {
        //        var treeNode = this.CatchClause.ToTreeNode();
        //        process.AddHighlighting(treeNode.CatchKeyword.GetDocumentRange(), new SwallowedExceptionsHighlighting(this));
        //    }

        //    foreach (var tryStatementModel in this.TryStatementModels)
        //    {
        //        tryStatementModel.AssignHighlights(process);
        //    }

        //    foreach (var throwStatementModel in this.ThrowStatementModels)
        //    {
        //        throwStatementModel.AssignHighlights(process);
        //    }
        //}

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);

            foreach (var tryStatementModel in this.TryStatementModels)
            {
                tryStatementModel.Accept(analyzerBase);
            }

            foreach (var throwStatementModel in this.ThrowStatementModels)
            {
                throwStatementModel.Accept(analyzerBase);
            }
        }
    }
}