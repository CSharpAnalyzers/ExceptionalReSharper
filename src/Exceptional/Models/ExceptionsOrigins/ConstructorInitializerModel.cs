using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models.ExceptionsOrigins
{
    internal class ConstructorInitializerModel : TreeElementModelBase<ITreeNode>, IExceptionsOriginModel
    {
        public ConstructorInitializerModel(IAnalyzeUnit analyzeUnit, IConstructorInitializer node, IBlockModel containingBlock)
            : base(analyzeUnit, node)
        {
            ContainingBlock = containingBlock;
            ThrownExceptions = node.Reference != null ?
                ThrownExceptionsReader.Read(analyzeUnit, this, node.Reference.Resolve().DeclaredElement) :
                new List<ThrownExceptionModel>();
        }

        public ConstructorInitializerModel(
            ConstructorDeclarationModel analyzeUnit,
            IConstructor constructor,
            ConstructorDeclarationModel containingBlock)
            : base(
                analyzeUnit,
                analyzeUnit.Node.TypeName
            )
        {
            ContainingBlock = containingBlock;
            ThrownExceptions = ThrownExceptionsReader.Read(analyzeUnit, this, constructor);
        }

        /// <summary>Gets the list of exception which can be thrown from this object. </summary>
        public IEnumerable<ThrownExceptionModel> ThrownExceptions { get; private set; }

        /// <summary>Gets the parent block which contains this block. </summary>
        public IBlockModel ContainingBlock { get; private set; }

        /// <summary>Creates a try-catch block around this block. </summary>
        /// <param name="exceptionType">The exception type to catch. </param>
        /// <returns><c>true</c> if the try-catch block could be created; otherwise, <c>false</c>. </returns>
        public bool SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            throw new System.NotSupportedException();
        }

        /// <summary>Analyzes the object and its children. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var thrownException in ThrownExceptions)
                thrownException.Accept(analyzer);
            base.Accept(analyzer);
        }
    }
}