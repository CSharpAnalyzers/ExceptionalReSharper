using System.Security;

namespace Exceptional.Playground.Issues
{
    // https://exceptional.codeplex.com/workitem/11011

    public interface IInterfaceAbstractExceptions
    {
        /// <summary>Fooes this instance.</summary>
        /// <exception cref="SecurityException">Test. </exception>
        void Foo();
        // No warning
    }

    public abstract class InterfaceAbstractExceptions
    {
        /// <summary>Fooes this instance.</summary>
        /// <exception cref="SecurityException">Test. </exception>
        public abstract void Foo();
        // No warning
    }
}
