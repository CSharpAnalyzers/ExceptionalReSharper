namespace Exceptional.Playground.Fixed
{
    public class OverflowException
    {
        public void Test01(int[] array)
        {
            // one warning on array.Length, that OverflowException is not documented shall be shown
            var length = array.Length;
        }

        /// <exception cref="T:System.OverflowException">
        ///     The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue" /> elements.
        /// </exception>
        public void Test02(int[] array)
        {
            // no warning shall be shown on array.Length
            var length = array.Length;
        }
    }
}