// Copyright (c) 2009-2010 Cofinite Solutions. All rights reserved.
using CodeGears.ReSharper.Exceptional.Analyzers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;

namespace CodeGears.ReSharper.Exceptional.Model
{
    internal class ThrownExceptionModel : ModelBase
    {
        public bool IsCatched { get; private set; }
        public bool IsDocumented { get; private set; }

        public IDeclaredType ExceptionType { get; private set; }
        public IExceptionsOriginModel Parent { get; private set; }

        public override DocumentRange DocumentRange
        {
            get { return Parent.DocumentRange; }
        }

        public ThrownExceptionModel(IAnalyzeUnit analyzeUnit, IDeclaredType exceptionType, IExceptionsOriginModel parent)
            : base(analyzeUnit)
        {
            ExceptionType = exceptionType;
            Parent = parent;

            IsCatched = CheckIfCatched();
            IsDocumented = CheckIfDocumented();
        }

        private bool CheckIfDocumented()
        {
            var docCommentBlockNode = AnalyzeUnit.DocCommentBlockModel;
            if (docCommentBlockNode == null) return false;

            foreach (var exceptionDocumentationModel in docCommentBlockNode.ExceptionDocCommentModels)
            {
                if (Throws(exceptionDocumentationModel.ExceptionType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckIfCatched()
        {
            if (ExceptionType == null) return false;

            return Parent.ContainingBlockModel.CatchesException(ExceptionType);
        }

        /// <summary>Checks whether this thrown exception equals to <paramref name="exceptionType"/>.</summary>
        public bool Throws(IDeclaredType exceptionType)
        {
            if (ExceptionType == null) return false;
            if (exceptionType == null) return false;

            return ExceptionType.GetClrName().ShortName.Equals(exceptionType.GetClrName().ShortName);
        }

        public override void Accept(AnalyzerBase analyzerBase)
        {
            analyzerBase.Visit(this);
        }
    }
}