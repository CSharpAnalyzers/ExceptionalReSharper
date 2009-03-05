using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    public class TryStatementModel : ModelBase
    {
        public ITryStatement TryStatement { get; set; }

        public List<CatchClauseModel> CatchClauseModels { get; set; }

        public TryStatementModel(ITryStatement tryStatement)
        {
            TryStatement = tryStatement;
            CatchClauseModels = new List<CatchClauseModel>();

            InitializeCatchClauses();
        }

        private void InitializeCatchClauses()
        {
            foreach (var catchClause in this.TryStatement.Catches)
            {
                CatchClauseModels.Add(new CatchClauseModel(catchClause));
            }
        }

        public bool Catches(IDeclaredType exception)
        {
            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                if(catchClauseModel.Catches(exception))
                {
                    return true;
                }
            }

            return false;
        }
    }
}