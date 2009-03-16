using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Naming;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal abstract class CatchClauseModel : ModelBase, IBlockModel
    {
        protected ICatchClauseNode CatchClauseNode { get; set; }
        public CatchVariableModel VariableModel { get; set; }
        public bool IsCatchAll { get; private set; }
        public bool IsRethrown { get; set; }

        public abstract bool Catches(IDeclaredType exception);
        public abstract bool HasExceptionType { get; }
        
        public bool HasVariable
        {
            get { return this.VariableModel != null; }
        }

        public override DocumentRange DocumentRange
        {
            get { return this.CatchClauseNode.CatchKeyword.GetDocumentRange(); }
        }

        protected CatchClauseModel(MethodDeclarationModel methodDeclarationModel, ICatchClauseNode catchClauseNode)
            : base(methodDeclarationModel)
        {
            CatchClauseNode = catchClauseNode;
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
            if(this.CatchClauseNode.ExceptionType == null) return false;

            return this.CatchClauseNode.ExceptionType.GetCLRName().Equals("System.Exception");
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

        #region IBlockModel implementation

        public List<ThrowStatementModel> ThrowStatementModels { get; private set; }
        public List<TryStatementModel> TryStatementModels { get; private set; }
        public IBlockModel ParentBlock { get; set; }

        public abstract IDeclaredType GetCatchedException();

        public IEnumerable<ThrownExceptionModel> ThrownExceptionModelsNotCatched
        {
            get
            {
                foreach (var throwStatementModel in this.ThrowStatementModels)
                {
                    foreach (var thrownExceptionModel in throwStatementModel.ThrownExceptions)
                    {
                        if (thrownExceptionModel.IsCatched == false)
                        {
                            yield return thrownExceptionModel;
                        }
                    }
                }

                for (var i = 0; i < this.TryStatementModels.Count; i++)
                {
                    IBlockModel tryStatementModel = this.TryStatementModels[i];
                    foreach (var model in tryStatementModel.ThrownExceptionModelsNotCatched)
                    {
                        yield return model;
                    }
                }
            }
        }

        public bool CatchesException(IDeclaredType exception)
        {
            return this.ParentBlock.CatchesException(exception);
        }
        #endregion

        public abstract void AddCatchVariable(string variableName);

        public string SuggestVariableName()
        {
            var namesCollection = NameSuggestionUtil.CreateEmptyCollection(
                this.CatchClauseNode.Language, PluralityKinds.Single, true, this.CatchClauseNode.GetSolution());
            namesCollection.Add(this.GetCatchedException(), PluralityKinds.Single);

            var roots = new List<NameRoot>(namesCollection.GetRoots("e", PredefinedPrefixPolicy.Preserve));
            var newNames = new List<string>(NamingManager.SuggestUniqueShortNames(roots, this.CatchClauseNode, NamedElementKinds.Locals, ScopeKind.LocalSelfScoped));

            return newNames.Count > 0 ? newNames.GetLast() : "exception";
        }
    }
}