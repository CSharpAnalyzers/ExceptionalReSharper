using System;
using System.IO;

namespace Exceptional.Playground
{
    public class HideWarningWhenBaseExceptionAlreadyDocumented
    {
        /// <exception cref="ArgumentException">Test. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode" /> specified an invalid value. </exception>
        /// <exception cref="NotSupportedException"><paramref name="path" /> is in an invalid format. </exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="UnauthorizedAccessException"><paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or- <paramref name="path" /> specified a directory.-or- The caller does not have the required permission. -or-<paramref name="mode" /> is <see cref="F:System.IO.FileMode.Create" /> and the specified file is a hidden file.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive). </exception>
        public void Test1(int i)
        {
            File.Open("a", FileMode.Open); // Hint: When IsDocumentationOfExceptionSubtypeSufficientForThrowStatements is enabled
        }

        /// <exception cref="ArgumentException">Test. </exception>
        public void Test2(int i)
        {
            if (i == 0)
                throw new ArgumentNullException(); // Hint: When IsDocumentationOfExceptionSubtypeSufficientForReferenceExpressions is enabled
            throw new ArgumentException();
        }
    }
}
