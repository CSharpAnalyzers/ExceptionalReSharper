using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models.ExceptionsOrigins
{
    internal class ReferenceExpressionModel : ExpressionExceptionsOriginModelBase<IReferenceExpression>
    {
        private IEnumerable<ThrownExceptionModel> _thrownExceptions;
        private bool? _isInvocation = null;
        private bool? _isEventInvocation = null;
        private IAssignmentExpression _assignment = null;

        public ReferenceExpressionModel(IAnalyzeUnit analyzeUnit, IReferenceExpression invocationExpression, IBlockModel containingBlock)
            : base(analyzeUnit, invocationExpression, containingBlock)
        {
        }

        /// <summary>Gets the document range of this block. </summary>
        public override DocumentRange DocumentRange
        {
            get
            {
                if (Node.Parent is IElementAccessExpression)
                    return Node.Parent.GetExtendedDocumentRange();

                //if (Node.Parent is IAssignmentExpression)
                //    return Node.Parent.GetExtendedDocumentRange();

                return Node.Reference.GetDocumentRange();
            }
        }

        /// <summary>Gets a list of exceptions which may be thrown from this reference expression (empty if <see cref="IsInvocation"/> is false). </summary>
        public override IEnumerable<ThrownExceptionModel> ThrownExceptions
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
                    if (!(Node.Parent is IAssignmentExpression) && IsInvocation)
                    {
                        var psiModule = Node.GetPsiModule();

                        var delegateType = TypeFactory.CreateTypeByCLRName("System.Delegate", psiModule);

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

        public bool IsExceptionValid(ExceptionDocCommentModel comment)
        {
            if (comment.Accessor.IsNullOrWhitespace())
                return true;

            if (this.IsInvocation == false)
                return false;

            if (_assignment != null)
            {
                var exceptionOrigin = comment.AssociatedExceptionModel.ExceptionsOrigin.Node.GetText().TrimFromStart("this.");
                var assignmentDestination = _assignment.Dest.LastChild.GetText();
                if (assignmentDestination.Contains(exceptionOrigin) && comment.Accessor == "get")
                {
                    return false;
                }

                if (assignmentDestination.Contains(exceptionOrigin) == false && comment.Accessor == "set")
                {
                    return false;
                }
            }

            return true;
        }

        private ThrownExceptionModel CreateThrownSystemException()
        {
            var psiModule = Node.GetPsiModule();

            var systemExceptionType = TypeFactory.CreateTypeByCLRName("System.Exception", psiModule);

            string accessor = null;
            if (ContainingBlock is AccessorDeclarationModel)
                accessor = ((AccessorDeclarationModel)ContainingBlock).Node.NameIdentifier.Name;

            var thrownException = new ThrownExceptionModel(AnalyzeUnit, this, systemExceptionType, 
                "A delegate callback throws an exception.", true, accessor);

            return thrownException;
        }

        private bool IsInvocationInternal()
        {
            var parent = Node.Parent;
            while (parent != null)
            {
                if (parent == AnalyzeUnit.Node)
                    return false;

                if (parent is IAssignmentExpression || 
                    parent is ICSharpArgument || 
                    parent is IExpressionInitializer || 
                    parent is IAccessorDeclaration)
                {
                    var property = Node.Reference.Resolve().DeclaredElement as IProperty;
                    if (parent is IAssignmentExpression)
                        _assignment = (IAssignmentExpression)parent;

                    if (property != null)
                    {
                        var psiModule = Node.GetPsiModule();
                        var delegateType = TypeFactory.CreateTypeByCLRName("System.Delegate", psiModule);
                        return !property.Type.IsSubtypeOf(delegateType);
                    }
                    return false;
                }
                
                if (parent is IElementAccessExpression ||
                    parent is IInvocationExpression)
                    return true;

                parent = parent.Parent;
            }
            return false;
        }
    }
}