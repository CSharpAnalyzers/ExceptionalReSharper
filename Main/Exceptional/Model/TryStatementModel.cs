/// <copyright file="TryStatementModel.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System.Collections.Generic;
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class TryStatementModel : ModelBase, IBlockModel
    {
        private ITryStatement TryStatement { get; set; }
        
        public List<CatchClauseModel> CatchClauseModels { get; private set; }
        public List<ThrowStatementModel> ThrowStatementModels { get; private set; }
        public List<TryStatementModel> TryStatementModels { get; private set; }
        public IBlockModel ParentBlock { get; set; }

        public TryStatementModel(MethodDeclarationModel methodDeclarationModel, ITryStatement tryStatement)
            : base(methodDeclarationModel)
        {
            TryStatement = tryStatement;
            CatchClauseModels = new List<CatchClauseModel>();
            ThrowStatementModels = new List<ThrowStatementModel>();
            TryStatementModels = new List<TryStatementModel>();
        }

        public bool CatchesException(IDeclaredType exception)
        {
            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                if(catchClauseModel.Catches(exception))
                {
                    return true;
                }
            }

            return this.ParentBlock.CatchesException(exception);
        }

        public IDeclaredType GetCatchedException()
        {
            return this.ParentBlock.GetCatchedException();
        }

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

                for (var i = 0; i < this.CatchClauseModels.Count; i++)
                {
                    IBlockModel catchClauseModel = this.CatchClauseModels[i];
                    foreach (var model in catchClauseModel.ThrownExceptionModelsNotCatched)
                    {
                        yield return model;
                    }
                }
            }
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            foreach (var innerTryStatementModel in this.TryStatementModels)
            {
                innerTryStatementModel.Accept(analyzerBase);
            }

            foreach (var catchClauseModel in this.CatchClauseModels)
            {
                catchClauseModel.Accept(analyzerBase);
            }

            foreach (var throwStatementModel in this.ThrowStatementModels)
            {
                throwStatementModel.Accept(analyzerBase);
            }
        }
    }
}