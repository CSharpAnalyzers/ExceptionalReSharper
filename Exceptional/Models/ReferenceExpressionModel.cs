using System;
using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate.MemberBody;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Utilities;

namespace ReSharper.Exceptional.Models
{
    internal class ReferenceExpressionModel : TreeElementModelBase<IReferenceExpression>, IExceptionsOriginModel
    {
        private IEnumerable<ThrownExceptionModel> _thrownExceptions;
        private bool? _isInvocation = null;
        private bool? _isEventInvocation = null;

        public ReferenceExpressionModel(IAnalyzeUnit analyzeUnit, IReferenceExpression invocationExpression, IBlockModel containingBlock)
            : base(analyzeUnit, invocationExpression)
        {
            ContainingBlock = containingBlock;
        }

        /// <summary>Gets the parent block which contains this block. </summary>
        public IBlockModel ContainingBlock { get; private set; }

        /// <summary>Gets the document range of this block. </summary>
        public override DocumentRange DocumentRange
        {
            get { return Node.Reference.GetDocumentRange(); }
        }

        /// <summary>Gets a list of exceptions which may be thrown from this reference expression (empty if <see cref="IsInvocation"/> is false). </summary>
        public IEnumerable<ThrownExceptionModel> ThrownExceptions
        {
            get
            {
                if (_thrownExceptions == null)
                {
                    if (IsDelegateInvocation)
                    {
                        var thrownException = CreateThrownSystemException();
                        _thrownExceptions = new List<ThrownExceptionModel> { thrownException };
                    }
                    else if (IsInvocation)
                        _thrownExceptions = ThrownExceptionsReader.Read(AnalyzeUnit, this, Node);
                    else
                        _thrownExceptions = new List<ThrownExceptionModel>();
                }
                return _thrownExceptions;
            }
        }

        /// <summary>Gets a value indicating whether this is a method, event or property invocation. </summary>
        public bool IsInvocation
        {
            get
            {
                if (!_isInvocation.HasValue)
                    _isInvocation = IsInvocationInternal();
                return _isInvocation.Value;
            }
        }

        /// <summary>Gets a value indicating whether this is an delegate invocation. </summary>
        public bool IsDelegateInvocation
        {
            get
            {
                if (!_isEventInvocation.HasValue)
                {
                    if (IsInvocation)
                    {
                        var psiModule = Node.GetPsiModule();
                        var delegateType = TypeFactory.CreateTypeByCLRName("System.Delegate", psiModule, psiModule.GetContextFromModule());

                        var type = Node.GetExpressionType().ToIType();
                        if (type != null)
                            _isEventInvocation = type.IsSubtypeOf(delegateType);
                        else
                            _isEventInvocation = false;
                    }
                    else
                        _isEventInvocation = false;
                }
                return _isEventInvocation.Value;
            }
        }

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            foreach (var thrownExceptionModel in ThrownExceptions)
                thrownExceptionModel.Accept(analyzer);
        }

        /// <summary>Creates a try-catch block around this block. </summary>
        /// <param name="exceptionType">The exception type to catch. </param>
        /// <returns><c>true</c> if the try-catch block could be created; otherwise, <c>false</c>. </returns>
        public bool SurroundWithTryBlock(IDeclaredType exceptionType)
        {
            var containingStatement = Node.GetContainingStatement();
            if (containingStatement != null && containingStatement.LastChild != null)
            {
                var codeElementFactory = new CodeElementFactory(GetElementFactory());
                var exceptionVariableName = NameFactory.CatchVariableName(Node, exceptionType);
                var tryStatement = codeElementFactory.CreateTryStatement(exceptionType, exceptionVariableName);

                var spaces = GetElementFactory().CreateWhitespaces(Environment.NewLine);

                LowLevelModificationUtil.AddChildAfter(containingStatement.LastChild, spaces[0]);

                var block = codeElementFactory.CreateBlock(containingStatement);
                tryStatement.SetTry(block);

                containingStatement.ReplaceBy(tryStatement);
                return true;
            }
            return false;
        }

        private ThrownExceptionModel CreateThrownSystemException()
        {
            var psiModule = Node.GetPsiModule();
            var systemExceptionType = TypeFactory.CreateTypeByCLRName("System.Exception", psiModule,
                psiModule.GetContextFromModule());
            var thrownException = new ThrownExceptionModel(
                AnalyzeUnit, this, systemExceptionType, "A delegate callback throws an exception. ", true);
            return thrownException;
        }

        private bool IsInvocationInternal()
        {
            var parent = Node.Parent;
            while (parent != null)
            {
                if (parent == AnalyzeUnit.Node)
                    return false;

                if (parent is ICSharpArgument || parent is IExpressionInitializer)
                {
                    var property = Node.Reference.Resolve().DeclaredElement as IProperty;
                    if (property != null)
                    {
                        var psiModule = Node.GetPsiModule();
                        var delegateType = TypeFactory.CreateTypeByCLRName("System.Delegate", psiModule, psiModule.GetContextFromModule());
                        return !property.Type.IsSubtypeOf(delegateType);
                    }
                    return false;
                }

                if (parent is IInvocationExpression)
                    return true;

                parent = parent.Parent;
            }
            return false;
        }
    }
}