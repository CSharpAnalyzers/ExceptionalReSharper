using System;

namespace ExceptionalSandBox
{
    public interface ITest
    {
        /// <summary></summary>
        /// <exception cref="InvalidOperationException" />
        void ThrowExeption();
    }
}