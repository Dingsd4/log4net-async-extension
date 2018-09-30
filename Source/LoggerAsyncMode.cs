namespace log4net
{
    /// <summary>
    /// Provides available modes for the <see cref="LoggerAsync"/>
    /// </summary>
    public enum LoggerAsyncMode
    {
        /// <summary>The undefined mode</summary>
        Undefined = 0,

        /// <summary>
        /// Opportune logging: The <see cref="LoggerAsync"/> may discard old messages to keep up
        /// </summary>
        Opportune,

        /// <summary>
        /// Continuous logging: The <see cref="LoggerAsync"/> caches all messages and write them out whenever possible
        /// </summary>
        Continuous,
    }
}