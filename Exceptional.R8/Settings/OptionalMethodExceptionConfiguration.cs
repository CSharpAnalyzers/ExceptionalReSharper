using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util.Logging;

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

        public string FullMethodName { get; private set; }

        public string ExceptionType { get; private set; }

        public IDeclaredType GetExceptionType(ExceptionalDaemonStageProcess process)
        {
            if (_exceptionTypeLoaded)
                return _exceptionType;

            try
            {
                _exceptionType = TypeFactory.CreateTypeByCLRName(ExceptionType, process.PsiModule, process.PsiModule.GetContextFromModule());
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