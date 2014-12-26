using System.Linq;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;
using ReSharper.Exceptional.Settings;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes an exception documentation and checks if it is thrown from the documented element.</summary>
    internal class IsDocumentedExceptionThrownAnalyzer : AnalyzerBase
    {
        /// <summary>Initializes a new instance of the <see cref="AnalyzerBase"/> class. </summary>
        /// <param name="process">The process. </param>
        /// <param name="settings">The settings. </param>
        public IsDocumentedExceptionThrownAnalyzer(ExceptionalDaemonStageProcess process, ExceptionalSettings settings)
            : base(process, settings) { }

        /// <summary>Performs analyze of <paramref name="exceptionDocumentation"/>.</summary>
        /// <param name="exceptionDocumentation">Exception documentation to analyze.</param>
        public override void Visit(ExceptionDocCommentModel exceptionDocumentation)
        {
            if (exceptionDocumentation == null)
                return;

            if (IsAbstractOrInterfaceMethod(exceptionDocumentation))
                return;

            if (!exceptionDocumentation.AnalyzeUnit.IsInspectionRequired)
                return;

            if (IsDocumentedExceptionThrown(exceptionDocumentation))
                return;

            var isOptional = IsDocumentedExceptionOrSubtypeThrown(exceptionDocumentation);

            var highlighting = isOptional
                ? new ExceptionNotThrownOptionalHighlighting(exceptionDocumentation)
                : new ExceptionNotThrownHighlighting(exceptionDocumentation);

            Process.Hightlightings.Add(new HighlightingInfo(exceptionDocumentation.DocumentRange, highlighting, null, null));
        }

        private bool IsAbstractOrInterfaceMethod(ExceptionDocCommentModel exceptionDocumentation)
        {
            if (exceptionDocumentation.AnalyzeUnit is MethodDeclarationModel)
            {
                var declaredElement = ((MethodDeclarationModel)exceptionDocumentation.AnalyzeUnit).Node.DeclaredElement;
                if (declaredElement != null && declaredElement.IsAbstract)
                    return true;
            }
            return false;
        }

        private bool IsDocumentedExceptionThrown(ExceptionDocCommentModel exceptionDocumentation)
        {
            return exceptionDocumentation
                .AnalyzeUnit
                .UncaughtThrownExceptions
                .Any(m => m.IsException(exceptionDocumentation.ExceptionType));
        }

        private bool IsDocumentedExceptionOrSubtypeThrown(ExceptionDocCommentModel exceptionDocumentation)
        {
            return exceptionDocumentation
                .AnalyzeUnit
                .UncaughtThrownExceptions
                .Any(m => ThrowsExceptionOrSubtype(exceptionDocumentation, m));
        }

        private bool ThrowsExceptionOrSubtype(ExceptionDocCommentModel exceptionDocumentation, ThrownExceptionModel thrownException)
        {
            if (thrownException.IsThrownFromThrowStatement)
            {
                if (Settings.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements)
                    return thrownException.IsExceptionOrSubtype(exceptionDocumentation.ExceptionType);
            }
            else
            {
                if (Settings.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions)
                    return thrownException.IsExceptionOrSubtype(exceptionDocumentation.ExceptionType);
            }
            return false;
        }
    }
}