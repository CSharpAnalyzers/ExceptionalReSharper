using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util.Logging;

namespace ReSharper.Exceptional.Settings
{
    [SettingsKey(typeof(EnvironmentSettings), "Exceptional Settings")]
    public class ExceptionalSettings
    {
        [SettingsEntry(true, "Inspect public methods")]
        public bool InspectPublicMethods { get; set; }

        [SettingsEntry(true, "Inspect internal methods")]
        public bool InspectInternalMethods { get; set; }

        [SettingsEntry(true, "Inspect protected methods")]
        public bool InspectProtectedMethods { get; set; }

        [SettingsEntry(false, "Inspect private methods")]
        public bool InspectPrivateMethods { get; set; }


        [SettingsEntry(true, "Delegate invocations may throw System.Exception")]
        public bool DelegateInvocationsMayThrowExceptions { get; set; }


        [SettingsEntry(false, "Is documentation of exception subtype sufficient for throw statements")]
        public bool IsDocumentationOfExceptionSubtypeSufficientForThrowStatements { get; set; }

        [SettingsEntry(false, "Is documentation of exception subtype sufficient for reference expressions")]
        public bool IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions { get; set; }


        [SettingsEntry(DefaultOptionalExceptions, "Optional exceptions")]
        public string OptionalExceptions { get; set; }

        [SettingsEntry(true, "Use default optional exceptions")]
        public bool UseDefaultOptionalExceptions { get; set; }

        [SettingsEntry(DefaultOptionalMethodExceptions, "Optional method exceptions")]
        public string OptionalMethodExceptions { get; set; }

        [SettingsEntry(true, "Use default optional method exceptions")]
        public bool UseDefaultOptionalMethodExceptions { get; set; }


        private const string DefaultOptionalExceptions =
@"-- Contracts
System.ArgumentException,InvocationOnly
System.InvalidOperationException,InvocationOnly
System.FormatException,InvocationOnly

System.NotSupportedException,InvocationOnly
System.NotImplementedException,ThrowOnly

-- Unit testing
Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException,InvocationOnly";

        private const string DefaultOptionalMethodExceptions = 
@"System.Collections.IDictionary.Add,System.NotSupportedException
System.Nullable.Value,System.InvalidOperationException
System.Windows.DependencyObject.GetValue,System.InvalidOperationException
System.Windows.DependencyObject.SetValue,System.InvalidOperationException
System.Console.WriteLine,System.IO.IOException";

        private List<OptionalExceptionConfiguration> _optionalExceptionsCache = null;
        private List<OptionalMethodExceptionConfiguration> _optionalMethodExceptionsCache = null;

        public void InvalidateCaches()
        {
            lock (typeof(ExceptionalSettings))
            {
                _optionalExceptionsCache = null;
                _optionalMethodExceptionsCache = null;
            }
        }

        public IEnumerable<OptionalExceptionConfiguration> GetOptionalExceptions(ExceptionalDaemonStageProcess process)
        {
            if (_optionalExceptionsCache == null)
            {
                lock (typeof(ExceptionalSettings))
                {
                    if (_optionalExceptionsCache == null)
                        _optionalExceptionsCache = LoadOptionalExceptions(process);
                }
            }
            return _optionalExceptionsCache;
        }

        public IEnumerable<OptionalMethodExceptionConfiguration> GetOptionalMethodExceptions(ExceptionalDaemonStageProcess process)
        {
            if (_optionalMethodExceptionsCache == null)
            {
                lock (typeof(ExceptionalSettings))
                {
                    if (_optionalMethodExceptionsCache == null)
                        _optionalMethodExceptionsCache = LoadOptionalMethodExceptions();
                }
            }
            return _optionalMethodExceptionsCache;
        }

        private List<OptionalExceptionConfiguration> LoadOptionalExceptions(ExceptionalDaemonStageProcess process)
        {
            var list = new List<OptionalExceptionConfiguration>();
            var value = UseDefaultOptionalExceptions ? DefaultOptionalExceptions : OptionalExceptions;
            foreach (var line in value.Replace("\r", "").Split('\n').Where(n => !string.IsNullOrEmpty(n)))
            {
                var optionalException = TryLoadOptionalException(process, line);
                if (optionalException != null)
                    list.Add(optionalException);
            }
            return list;
        }

        private List<OptionalMethodExceptionConfiguration> LoadOptionalMethodExceptions()
        {
            var list = new List<OptionalMethodExceptionConfiguration>();
            var value = UseDefaultOptionalMethodExceptions ? DefaultOptionalMethodExceptions : OptionalMethodExceptions;
            foreach (var line in value.Replace("\r", "").Split('\n').Where(n => !string.IsNullOrEmpty(n)))
            {
                var excludedMethodException = TryLoadOptionalMethodException(line);
                if (excludedMethodException != null)
                    list.Add(excludedMethodException);
            }
            return list;
        }

        private static OptionalExceptionConfiguration TryLoadOptionalException(ExceptionalDaemonStageProcess process, string line)
        {
            try
            {
                var arr = line.Split(',');
                if (arr.Length == 2)
                {
                    var exceptionType = TypeFactory.CreateTypeByCLRName(arr[0], process.PsiModule, process.PsiModule.GetContextFromModule());

                    OptionalExceptionReplacementType replacementType;
                    if (Enum.TryParse(arr[1], out replacementType))
                        return new OptionalExceptionConfiguration(exceptionType, replacementType);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(string.Format("[Exceptional] Error loading optional exception '{0}'", line), ex);
            }
            return null;
        }

        private static OptionalMethodExceptionConfiguration TryLoadOptionalMethodException(string line)
        {
            var arr = line.Split(',');
            if (arr.Length == 2)
                return new OptionalMethodExceptionConfiguration(arr[0], arr[1]);
            return null;
        }
    }
}
