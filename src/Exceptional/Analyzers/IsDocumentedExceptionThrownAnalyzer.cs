using System.Linq;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using ReSharper.Exceptional.Highlightings;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Analyzers
{
    /// <summary>Analyzes an exception documentation and checks if it is thrown from the documented element.</summary>
    internal class IsDocumentedExceptionThrownAnalyzer : AnalyzerBase
    {
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

            ServiceLocator.StageProcess.AddHighlighting(highlighting, exceptionDocumentation.DocumentRange);
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
                .Any(m => m.IsException(exceptionDocumentation));
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
                if (ServiceLocator.Settings.IsDocumentationOfExceptionSubtypeSufficientForThrowStatements)
                    return thrownException.IsExceptionOrSubtype(exceptionDocumentation);
            }
            else
            {
                if (ServiceLocator.Settings.IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions)
                    return thrownException.IsExceptionOrSubtype(exceptionDocumentation);
            }
            return false;
        }
    }
}