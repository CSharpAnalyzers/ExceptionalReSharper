using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models.ExceptionsOrigins
{
    internal class ObjectCreationExpressionModel : ExpressionExceptionsOriginModelBase<IObjectCreationExpression>
    {
        private IEnumerable<ThrownExceptionModel> _thrownExceptions;

        public ObjectCreationExpressionModel(IAnalyzeUnit analyzeUnit, IObjectCreationExpression objectCreationExpression, IBlockModel containingBlock)
            : base(analyzeUnit, objectCreationExpression, containingBlock)
        {
        }

        /// <summary>Gets a list of exceptions which may be thrown from this reference expression. </summary>
        public override IEnumerable<ThrownExceptionModel> ThrownExceptions
        {
            get
            {
                if (_thrownExceptions == null)
                    _thrownExceptions = ThrownExceptionsReader.Read(AnalyzeUnit, this);
                return _thrownExceptions;
            }
        }

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var thrownExceptionModel in ThrownExceptions)
                thrownExceptionModel.Accept(analyzer);
        }
    }
}