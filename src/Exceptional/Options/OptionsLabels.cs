using System.Security.RightsManagement;

namespace ReSharper.Exceptional.Options
{
    public static class OptionsLabels
    {
        public static class InspectionLevel
        {
            public const string InspectPublicMethodsAndProperties = "Inspect public methods";
            public const string InspectInternalMethodsAndProperties = "Inspect internal methods";
            public const string InspectProtectedMethodsAndProperties = "Inspect protected methods";
            public const string InspectPrivateMethodsAndProperties = "Inspect private methods";
        }

        public static class General
        {
            public const string DelegateInvocationsMayThrowSystemException = "Delegate invocations may throw System.Exception";

            public const string DocumentationOfThrownExceptionsSubtypeHeader = "Documentation of thrown exception's subtype is sufficient...";
            public const string IsDocumentationOfExceptionSubtypeSufficientForThrowStatements = "... for throw statements";
            public const string IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions = "... for method or property invocations";
        }

        public static class ExceptionTypesAsHint
        {
            public const string Description = "Defines exception types which are shown as hints instead of warnings.";
            public const string UsePredefined = "Use predefined otional exceptions";
            public const string ShowPredefined = "Show predefined optional exceptions";
            public const string Note = "Format: ExceptionType,[Always|InvocationOnly|ThrowOnly]";
        }

        public static class ExceptionTypesAsHintForMethodOrProperty
        {
            public const string Description = "Define exceptions that are thrown from methods or properties which are shown as hints instead of warnings.";
            public const string UsePredefined = "Use predefined optional method and property exceptions";
            public const string ShowPredefined = "Show predefined optional method and property exceptions";
            public const string Note = "Format: FullMethodOrPropertyPath,ExceptionType";
        }

        public static class AccessorOverrides
        {
            public const string Description = "Override the property accessors for thrown exceptions of existing types. ";
            public const string UsePredefined = "Use predefined accessor overrides";
            public const string ShowPredefined = "Show predefined accessor overrides";
            public const string Note = "Format: FullPropertyPath,ExceptionType,[get|set]";
        }
    }
}