using System;
using System.Security;

namespace Exceptional.Playground.Fixed
{
    class RemovingExceptionDocumentationRemovesTooMuch
    {
        /// <summary>Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>. </summary>
        /// <returns>true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false. </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.
        /// </param><exception cref="NullReferenceException">The <paramref name="obj"/> parameter is null. 
        /// asdfasdf</exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            throw new SecurityException(); // Warning
            // Removing the NullReferenceException also removes the <filterpriority> and the </param> on the same line
            return false;
        }

    }
}
