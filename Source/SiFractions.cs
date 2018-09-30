using System.Diagnostics.CodeAnalysis;

namespace log4net
{
    /// <summary>
    /// si unit fractions
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum SiFractions : int
    {
        /// <summary>
        /// Milli
        /// </summary>
        m = 1,

        /// <summary>
        /// Micro
        /// </summary>
        µ,

        /// <summary>
        /// Nano
        /// </summary>
        n,

        /// <summary>
        /// Pico
        /// </summary>
        p,

        /// <summary>
        /// Femto
        /// </summary>
        f,

        /// <summary>
        /// Atto
        /// </summary>
        a,

        /// <summary>
        /// Zepto
        /// </summary>
        z,

        /// <summary>
        /// Yocto
        /// </summary>
        y,
    }
}