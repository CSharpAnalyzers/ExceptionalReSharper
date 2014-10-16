using JetBrains.ReSharper.Psi;

namespace ReSharper.Exceptional.Models
{
    // TODO: Directly use ThrownExceptionModel instead?
    public class ThrownExceptionDocumentationModel
    {
        public ThrownExceptionDocumentationModel(IDeclaredType exceptionType, string exceptionDescription)
        {
            ExceptionType = exceptionType; 
            ExceptionDescription = exceptionDescription;
        }

        public IDeclaredType ExceptionType { get; private set; }

        public string ExceptionDescription { get; private set; }
    }
}