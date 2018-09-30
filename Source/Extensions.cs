using log4net.Core;

namespace log4net
{
    /// <summary>
    /// Provides Extensions to log4net
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Construct a new wrapper for the specified <see cref="ILog"/> instance.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> instance to wrap.</param>
        /// <remarks>
        /// <para>
        /// Construct a new wrapper for the specified <see cref="ILog"/> instance.
        /// </para>
        /// </remarks>
        public static ILog AsSync(this ILog log)
        {
            return new LogImpl(AsSync(log.Logger));
        }

        /// <summary>
        /// Construct a new wrapper for the specified <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance to wrap.</param>
        /// <remarks>
        /// <para>
        /// Construct a new wrapper for the specified <see cref="ILogger"/> instance.
        /// </para>
        /// </remarks>
        public static ILogger AsSync(this ILogger logger)
        {
            return logger is LoggerAsync result ? result : new LoggerAsync(logger);
        }
    }
}
