using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes throw statements and checks that exceptions thrown outside are documented.</summary>
    internal class IsThrownExceptionDocumentedAnalyzer : AnalyzerBase
    {
        /// <summary>Initializes a new instance of the <see cref="AnalyzerBase"/> class. </summary>
        /// <param name="process">The process. </param>
        /// <param name="settings">The settings. </param>
        public IsThrownExceptionDocumentedAnalyzer(ExceptionalDaemonStageProcess process, ExceptionalSettings settings)
            : base(process, settings)
        {
        }

        /// <summary>Performs analyze of <paramref name="thrownException"/>.</summary>
        /// <param name="thrownException">Thrown exception to analyze.</param>
        public override void Visit(ThrownExceptionModel thrownException)
        {
            if (thrownException == null)
                return;

            if (!thrownException.AnalyzeUnit.IsInspectionRequired || thrownException.IsCaught || thrownException.IsExceptionDocumented)
                return;

            var isOptional = IsSubtypeDocumented(thrownException) ||
                             IsThrownExceptionSubclassOfOptionalException(thrownException) ||
                             IsThrownExceptionThrownFromExcludedMethod(thrownException);

            if (thrownException.IsEventInvocationException)
            {
                if (Settings.DelegateInvocationsMayThrowExceptions)
                {
                    var highlighting = new EventExceptionNotDocumentedHighlighting(thrownException);
                    Process.Hightlightings.Add(new HighlightingInfo(thrownException.DocumentRange, highlighting, null));
                }
            }
            else
            {
                var highlighting = isOptional
                    ? new ExceptionNotDocumentedOptionalHighlighting(thrownException)
                    : new ExceptionNotDocumentedHighlighting(thrownException);
                Process.Hightlightings.Add(new HighlightingInfo(thrownException.DocumentRange, highlighting, null));
            }
        }

        private bool IsSubtypeDocumented(ThrownExceptionModel thrownException)
        {
            if (thrownException.IsThrownFromThrowStatement)
            {
                if (Settings.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements && thrownException.IsExceptionOrSubtypeDocumented)
                    return true;
            }
            else
            {
                if (Settings.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions && thrownException.IsExceptionOrSubtypeDocumented)
                    return true;
            }

            return false;
        }

        private bool IsThrownExceptionSubclassOfOptionalException(ThrownExceptionModel thrownExceptionModel)
        {
            var optionalExceptions = Settings.GetOptionalExceptions(Process);

            if (thrownExceptionModel.IsThrownFromThrowStatement)
                return optionalExceptions.Any(e => e.ReplacementType != OptionalExceptionReplacementType.InvocationOnly &&
                    thrownExceptionModel.ExceptionType.IsSubtypeOf(e.ExceptionType));
            else
                return optionalExceptions.Any(e => e.ReplacementType != OptionalExceptionReplacementType.ThrowOnly &&
                    thrownExceptionModel.ExceptionType.IsSubtypeOf(e.ExceptionType));
        }

        private bool IsThrownExceptionThrownFromExcludedMethod(ThrownExceptionModel thrownException)
        {
            var parent = thrownException.ExceptionsOrigin as ReferenceExpressionModel;
            if (parent != null)
            {
                var node = parent.Node;
                var resolveResult = node.Reference.CurrentResolveResult;
                if (resolveResult != null)
                {
                    var element = resolveResult.DeclaredElement as IMethod;
                    if (element != null)
                    {
                        // remove generic placeholders and method signature
                        var fullMethodName = Regex.Replace(element.XMLDocId.Substring(2), "(``[0-9]+)|(\\(.*?\\))", "");

                        var excludedMethods = Settings.GetOptionalMethodExceptions(Process);
                        return excludedMethods
                            .Any(t => t.FullMethodName == fullMethodName && IsSubtypeOfOptionalException(t, thrownException, Process));
                    }
                }
            }
            return false;
        }

        private bool IsSubtypeOfOptionalException(OptionalMethodExceptionConfiguration optionalMethodException,
            ThrownExceptionModel thrownException, ExceptionalDaemonStageProcess process)
        {
            var exceptionType = optionalMethodException.GetExceptionType(process);
            if (exceptionType == null)
                return false;

            return thrownException.ExceptionType.IsSubtypeOf(optionalMethodException.GetExceptionType(process));
        }
    }
}