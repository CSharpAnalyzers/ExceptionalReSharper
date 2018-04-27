using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
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
        private bool? _isWrongAccessor = null;

        public ThrownExceptionModel(IAnalyzeUnit analyzeUnit, IExceptionsOriginModel exceptionsOrigin,
            IDeclaredType exceptionType, string exceptionDescription, bool isEventInvocationException, string exceptionAccessor)
            : base(analyzeUnit)
        {
            ExceptionType = exceptionType;
            ExceptionDescription = exceptionDescription;
            ExceptionsOrigin = exceptionsOrigin;
            ExceptionAccessor = exceptionAccessor;

            IsEventInvocationException = isEventInvocationException;
            
            CheckAccessorOverride(exceptionsOrigin, exceptionType);
        }

        private void CheckAccessorOverride(IExceptionsOriginModel exceptionsOrigin, IDeclaredType exceptionType)
        {
            var doc = GetXmlDocId(exceptionsOrigin.Node);
            if (doc != null)
            {
                var fullMethodName = Regex.Replace(doc.Substring(2), "(`[0-9]+)|(\\(.*?\\))", ""); // TODO: merge with other
                var overrides = ServiceLocator.Settings.GetExceptionAccessorOverrides();
                var ov =
                    overrides.SingleOrDefault(
                        o => o.FullMethodName == fullMethodName && o.GetExceptionType().Equals(exceptionType));
                if (ov != null)
                    ExceptionAccessor = ov.ExceptionAccessor;
            }
        }

        private string GetXmlDocId(ITreeNode node)
        {
            if (node is IReferenceExpression)
            {
                var element = ((IReferenceExpression) node).Reference.Resolve().DeclaredElement;

                if (node.Parent is IElementAccessExpression)
                {
                    var elementAccessReference = ((IElementAccessExpression) node.Parent).ElementAccessReference;
                    var declaredElement = elementAccessReference.Resolve().DeclaredElement;
                    var xmlDocIdOwner = declaredElement as IXmlDocIdOwner;
                    if (xmlDocIdOwner != null)
                        return xmlDocIdOwner.XMLDocId;
                }

                var t = element as IXmlDocIdOwner;
                if (t != null)
                    return t.XMLDocId;

                return null;
            }

            var propertyDeclaration = node as IPropertyDeclaration;
            if (propertyDeclaration != null)
                return propertyDeclaration.DeclaredElement.XMLDocId;

            var indexerDeclaration = node as IIndexerDeclaration;
            if (indexerDeclaration != null)
                return indexerDeclaration.DeclaredElement.XMLDocId;

            var methodDeclaration = node as IMethodDeclaration;
            if (methodDeclaration != null && methodDeclaration.DeclaredElement != null)
                return methodDeclaration.DeclaredElement.XMLDocId;

            return null;
        }

        public string ExceptionAccessor { get; private set; }

        public bool IsEventInvocationException { get; set; }

        public IDeclaredType ExceptionType { get; private set; }

        public string ExceptionDescription { get; set; }

        public IExceptionsOriginModel ExceptionsOrigin { get; private set; }

        /// <summary>Gets the document range of this object. </summary>
        public override DocumentRange DocumentRange
        {
            get { return ExceptionsOrigin.DocumentRange; }
        }

        /// <summary>
        /// Gets a value indicating whether this exception is thrown from a throw statement.
        /// </summary>
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
                    _isCaught = ExceptionType != null && ExceptionsOrigin.ContainingBlock.CatchesException(ExceptionType);
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
                if (IsWrongAccessor)
                    return true;

                return docCommentBlockNode
                    .DocumentedExceptions
                    .Any(check);
            }
            else
                return false;
        }

        private bool IsWrongAccessor
        {
            get
            {
                if (_isWrongAccessor == null)
                {
                    var parent = ExceptionsOrigin.Node.Parent;

                    // property
                    if (ExceptionAccessor == "get" && parent is IAssignmentExpression && parent.FirstChild == ExceptionsOrigin.Node)
                        _isWrongAccessor = true;
                    else if (ExceptionAccessor == "set" && parent is IExpressionInitializer && parent.LastChild == ExceptionsOrigin.Node)
                        _isWrongAccessor = true;

                    // indexer
                    else if (ExceptionAccessor == "get" && parent is IElementAccessExpression)
                    {
                        parent = parent.Parent;
                        while (parent != null && parent.FirstChild == parent.LastChild)
                            parent = parent.Parent;

                        if (parent != null &&
                            parent.FirstChild != null &&
                            parent.FirstChild.Children().Contains(ExceptionsOrigin.Node))
                            _isWrongAccessor = true;
                    }
                    else if (ExceptionAccessor == "set" && parent is IElementAccessExpression)
                    {
                        parent = parent.Parent;
                        while (parent != null && parent.FirstChild == parent.LastChild)
                            parent = parent.Parent;

                        if (parent != null &&
                            parent.LastChild != null &&
                            parent.LastChild.LastChild != null &&
                            parent.LastChild.LastChild.Children().Contains(ExceptionsOrigin.Node))
                            _isWrongAccessor = true;
                    }

                    if (_isWrongAccessor == null)
                        _isWrongAccessor = false;

                }
                return _isWrongAccessor.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the thrown exception is coming from a re-throw statement.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a re-throw; otherwise, <c>false</c>.
        /// </value>
        public bool IsRethrow
        {
            get
            {
                return (ExceptionsOrigin as ThrowStatementModel)?.IsRethrow ?? false;
            }
        }

        public string FullName
        {
            get
            {
                return ExceptionType.GetClrName().FullName;
            }            
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
            if (exceptionDocumentation.Accessor != null && exceptionDocumentation.Accessor != ExceptionAccessor)
                return false;

            return IsException(exceptionDocumentation.ExceptionType);
        }

        public bool IsExceptionOrSubtype(ExceptionDocCommentModel exceptionDocumentation)
        {
            if (exceptionDocumentation.Accessor != null && exceptionDocumentation.Accessor != ExceptionAccessor)
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