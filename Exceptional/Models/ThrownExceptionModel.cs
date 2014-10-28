using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using ReSharper.Exceptional.Analyzers;

namespace ReSharper.Exceptional.Models
{
    internal class ThrownExceptionModel : ModelBase
    {
        private bool? _isCaught = null;
        private bool? _isExceptionDocumented = null;
        private bool? _isExceptionOrSubtypeDocumented = null;

        public ThrownExceptionModel(
            IAnalyzeUnit analyzeUnit,
            IExceptionsOriginModel exceptionsOrigin,
            IDeclaredType exceptionType,
            string exceptionDescription,
            bool isEventInvocationException)
            : base(analyzeUnit)
        {
            ExceptionType = exceptionType;
            ExceptionDescription = exceptionDescription;
            ExceptionsOrigin = exceptionsOrigin;
            IsEventInvocationException = isEventInvocationException;
        }

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
                {
                    var docCommentBlockNode = AnalyzeUnit.DocumentationBlock;
                    if (docCommentBlockNode != null)
                    {
                        _isExceptionDocumented = docCommentBlockNode
                            .DocumentedExceptions
                            .Any(m => IsException(m.ExceptionType));
                    }
                    else
                        _isExceptionDocumented = false;

                }
                return _isExceptionDocumented.Value;
            }
        }

        /// <summary>Gets a value indicating whether the exact same exception or a subtype is documented. </summary>
        public bool IsExceptionOrSubtypeDocumented
        {
            get
            {
                if (!_isExceptionOrSubtypeDocumented.HasValue)
                {
                    var docCommentBlockNode = AnalyzeUnit.DocumentationBlock;
                    if (docCommentBlockNode != null)
                    {
                        _isExceptionOrSubtypeDocumented = docCommentBlockNode
                            .DocumentedExceptions
                            .Any(m => IsExceptionOrSubtype(m.ExceptionType));
                    }
                    else
                        _isExceptionOrSubtypeDocumented = false;

                }
                return _isExceptionOrSubtypeDocumented.Value;
            }
        }

        /// <summary>Checks whether the thrown exception is <paramref name="exceptionType"/>.</summary>
        public bool IsException(IDeclaredType exceptionType)
        {
            if (ExceptionType == null)
                return false;

            if (exceptionType == null)
                return false;

            return ExceptionType.Equals(exceptionType);
        }

        /// <summary>Checks whether the thrown exception is a subtype or equal to <paramref name="exceptionType"/>.</summary>
        public bool IsExceptionOrSubtype(IDeclaredType exceptionType)
        {
            if (ExceptionType == null)
                return false;

            if (exceptionType == null)
                return false;

            return ExceptionType.IsSubtypeOf(exceptionType);
        }

        /// <summary>Runs the analyzer against all defined elements. </summary>
        /// <param name="analyzer">The analyzer. </param>
        public override void Accept(AnalyzerBase analyzer)
        {
            analyzer.Visit(this);
        }
    }
}