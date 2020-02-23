using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util.Logging;

namespace ReSharper.Exceptional.Settings
{
    public class ExceptionAccessorOverride
    {
        private IDeclaredType _exceptionType = null;
        private bool _exceptionTypeLoaded = false;

        public ExceptionAccessorOverride(string fullMethodName, string exceptionType, string exceptionAccessor)
        {
            FullMethodName = fullMethodName;
            ExceptionType = exceptionType;
            ExceptionAccessor = exceptionAccessor;
        }

        public string FullMethodName { get; private set; }

        public string ExceptionType { get; private set; }

        public string ExceptionAccessor { get; private set; }

        public IDeclaredType GetExceptionType()
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