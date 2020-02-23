using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util.Logging;
using ReSharper.Exceptional.Models;

namespace ReSharper.Exceptional.Settings
{
    public class OptionalMethodExceptionConfiguration
    {
        private IDeclaredType _exceptionType = null;
        private bool _exceptionTypeLoaded = false; 

        public OptionalMethodExceptionConfiguration(string fullMethodName, string exceptionType)
        {
            FullMethodName = fullMethodName;
            ExceptionType = exceptionType;
        }

        internal string FullMethodName { get; private set; }

        internal string ExceptionType { get; private set; }

        internal bool IsSupertypeOf(ThrownExceptionModel thrownException)
        {
            var exceptionType = GetExceptionType();
            if (exceptionType == null)
                return false;

            return thrownException.ExceptionType.IsSubtypeOf(exceptionType);
        }

        private IDeclaredType GetExceptionType()
        {
            if (_exceptionTypeLoaded)
                return _exceptionType;

            try
            {
                _exceptionType = TypeFactory.CreateTypeByCLRName(ExceptionType, ServiceLocator.StageProcess.PsiModule);
            }
            catch (Exception ex)
            {
                Logger.LogException(string.Format("[Exceptional] Error loading excluded method exception '{0}'", ExceptionType), ex);
            }
            finally
            {
                _exceptionTypeLoaded = true; 
            }
            return _exceptionType;
        }
    }
}