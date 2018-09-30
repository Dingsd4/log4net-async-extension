using System.Diagnostics.CodeAnalysis;

namespace log4net
{
    /// <summary>
    /// Provides the international system of units default units
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum SiUnits : int
    {
        /// <summary>
        /// kilo
        /// </summary>
        k = 1,

        /// <summary>
        /// Mega
        /// </summary>
        M,

        /// <summary>
        /// Giga
        /// </summary>
        G,

        /// <summary>
        /// Tera
        /// </summary>
        T,

        /// <summary>
        /// Peta
        /// </summary>
        P,

        /// <summary>
        /// Exa
        /// </summary>
        E,

        /// <summary>
        /// Zetta
        /// </summary>
        Z,

        /// <summary>
        /// Yota
        /// </summary>
        Y,
    }
}