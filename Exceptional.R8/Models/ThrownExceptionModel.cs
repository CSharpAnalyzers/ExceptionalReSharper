using System;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Exceptional.Analyzers;
using ReSharper.Exceptional.Models.ExceptionsOrigins;

namespace ReSharper.Exceptional.Models
{
    internal class ThrownExceptionModel : ModelBase
    {
        private bool? _isCaught = null;
        private bool? _isExceptionDocumented = null;
        private bool? _isExceptionOrSubtypeDocumented = null;
        private bool? _isThrownFromAnonymousMethod = null;

        public ThrownExceptionModel(IAnalyzeUnit analyzeUnit, IExceptionsOriginModel exceptionsOrigin,
            IDeclaredType exceptionType, string exceptionDescription, bool isEventInvocationException, string accessor)
            : base(analyzeUnit)
        {
            ExceptionType = exceptionType;
            ExceptionDescription = exceptionDescription;
            ExceptionsOrigin = exceptionsOrigin;

            Accessor = accessor;
            IsEventInvocationException = isEventInvocationException;
        }

        public string Accessor { get; private set; }

        public bool IsEventInvocationException { get; set; }

        public IDeclaredType ExceptionType { get; private set; }

        public string ExceptionDescription { get; set; }

        public IExceptionsOriginModel ExceptionsOrigin { get; private set; }

        /// <summary>Gets the document range of this object. </summary>
        public override DocumentRange DocumentRange
        {
            get { return ExceptionsOrigin.DocumentRange; }
        }

        /// <summary>Gets a value indicating whether this exception is thrown from a throw statement. </summary>
        public bool IsThrownFromThrowStatement
        {
            get { return ExceptionsOrigin is ThrowStatementModel; }
        }

        public bool IsThrownFromAnonymousMethod
        {
            get
            {
                if (!_isThrownFromAnonymousMethod.HasValue)
                {
                    var parent = ExceptionsOrigin.Node;
                    _isThrownFromAnonymousMethod = IsParentAnonymousMethodExpression(parent);
                }
                return _isThrownFromAnonymousMethod.Value;
            }
        }

        public bool IsCaught
        {
            get
            {
                if (!_isCaught.HasValue)
                {
                    if (ExceptionType != null)
                        _isCaught = ExceptionsOrigin.ContainingBlock.CatchesException(ExceptionType);
                    else
                        _isCaught = false;
                }
                return _isCaught.Value;
            }
        }

        /// <summary>Gets a value indicating whether the exact same exception is documented. </summary>
        public bool IsExceptionDocumented
        {
            get
            {
                if (!_isExceptionDocumented.HasValue)
                    _isExceptionDocumented = IsExceptionDocumentedInternal(IsException);
                return _isExceptionDocumented.Value;
            }
        }

        /// <summary>Gets a value indicating whether the exact same exception or a subtype is documented. </summary>
        public bool IsExceptionOrSubtypeDocumented
        {
            get
            {
                if (!_isExceptionOrSubtypeDocumented.HasValue)
                    _isExceptionOrSubtypeDocumented = IsExceptionDocumentedInternal(IsExceptionOrSubtype);
                return _isExceptionOrSubtypeDocumented.Value;
            }
        }

        private bool IsExceptionDocumentedInternal(Func<ExceptionDocCommentModel, bool> check)
        {
            var docCommentBlockNode = AnalyzeUnit.DocumentationBlock;
            if (docCommentBlockNode != null)
            {
                var parent = ExceptionsOrigin.Node.Parent;

                // property
                if (Accessor == "get" &&
                    parent is IAssignmentExpression &&
                    parent.FirstChild == ExceptionsOrigin.Node)
                {
                    return true; // no warning
                }

                if (Accessor == "set" &&
                    parent is IExpressionInitializer &&
                    parent.LastChild == ExceptionsOrigin.Node)
                {
                    return true; // no warning
                }

                // indexer
                if (Accessor == "get" && parent is IElementAccessExpression)
                {
                    parent = parent.Parent;
                    while (parent != null && parent.FirstChild == parent.LastChild)
                        parent = parent.Parent;

                    if (parent != null && parent.FirstChild != null && parent.FirstChild.FirstChild == ExceptionsOrigin.Node)
                        return true; // no warning
                }

                if (Accessor == "set" && parent is IElementAccessExpression)
                {
                    parent = parent.Parent;
                    while (parent != null && parent.FirstChild == parent.LastChild)
                        parent = parent.Parent;

                    if (parent != null && parent.LastChild != null && parent.LastChild.LastChild != null &&
                        parent.LastChild.LastChild.FirstChild == ExceptionsOrigin.Node)
                        return true; // no warning
                }

                return docCommentBlockNode
                    .DocumentedExceptions
                    .Any(check);
            }
            else
                return false;
        }

        public bool IsException(IDeclaredType exceptionType)
        {
            if (ExceptionType == null)
                return false;

            if (exceptionType == null)
                return false;

            return ExceptionType.Equals(exceptionType);
        }

        /// <summary>Checks whether the thrown exception is the same as <paramref name="exceptionDocumentation"/>.</summary>
        public bool IsException(ExceptionDocCommentModel exceptionDocumentation)
        {
            if (exceptionDocumentation.Accessor != null && exceptionDocumentation.Accessor != Accessor)
                return false;

            return IsException(exceptionDocumentation.ExceptionType);
        }

        /// <summary>Checks whether the thrown exception is a subtype or equal to <paramref name="exceptionType"/>.</summary>
        public bool IsExceptionOrSubtype(ExceptionDocCommentModel exceptionDocumentation)
        {
            if (exceptionDocumentation.Accessor != null && exceptionDocumentation.Accessor != Accessor)
                return false;

            if (ExceptionType == null)
                return false;

            if (exceptionDocumentation.ExceptionType == null)
                return false;

            return ExceptionType.IsSubtypeOf(exceptionDocumentation.ExceptionType);
        }

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            analyzer.Visit(this);
        }

        private bool IsParentAnonymousMethodExpression(ITreeNode parent)
        {
            while (parent != null)
            {
                if (parent is IAnonymousMethodExpression || parent is IAnonymousFunctionExpression)
                    return true;
                parent = parent.Parent;
            }
            return false;
        }
    }
}