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
        public IsThrownExceptionDocumentedAnalyzer(ExceptionalDaemonStageProcess process, ExceptionalSettings settings)
            : base(process, settings)
        {
        }

        public override void Visit(ThrownExceptionModel thrownExceptionModel)
        {
            if (thrownExceptionModel == null)
                return;
            if (thrownExceptionModel.AnalyzeUnit.IsInspected == false)
                return;
            if (thrownExceptionModel.IsCaught)
                return;
            if (thrownExceptionModel.IsDocumented)
                return;

            if (IsThrownExceptionSubclassOfOptionalException(thrownExceptionModel) || IsThrownExceptionThrownFromExcludedMethod(thrownExceptionModel))
                Process.Hightlightings.Add(new HighlightingInfo(thrownExceptionModel.DocumentRange, new ExceptionNotDocumentedOptionalHighlighting(thrownExceptionModel), null, null));
            else
                Process.Hightlightings.Add(new HighlightingInfo(thrownExceptionModel.DocumentRange, new ExceptionNotDocumentedHighlighting(thrownExceptionModel), null, null));
        }

        private bool IsThrownExceptionSubclassOfOptionalException(ThrownExceptionModel thrownExceptionModel)
        {
            var optionalExceptions = Settings.GetOptionalExceptions(Process);

            if (thrownExceptionModel.Parent is ThrowStatementModel)
                return optionalExceptions.Any(e => e.ReplacementType != OptionalExceptionReplacementType.InvocationOnly && 
                    thrownExceptionModel.ExceptionType.IsSubtypeOf(e.ExceptionType));
            else
                return optionalExceptions.Any(e => e.ReplacementType != OptionalExceptionReplacementType.ThrowOnly &&
                    thrownExceptionModel.ExceptionType.IsSubtypeOf(e.ExceptionType));
        }

        private bool IsThrownExceptionThrownFromExcludedMethod(ThrownExceptionModel thrownExceptionModel)
        {
            var parent = thrownExceptionModel.Parent as ReferenceExpressionModel;
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
                        return excludedMethods.Any(t => t.FullMethodName == fullMethodName && thrownExceptionModel.IsSubtypeOf(t, Process));
                    }
                }
            }
            return false;
        }
    }
}