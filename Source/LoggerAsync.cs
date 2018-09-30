using log4net.Core;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Threading;

namespace log4net
{
    class LoggerAsync : ILogger
    {
        /// <summary>
        /// Gets / sets the operation mode.
        /// </summary>
        public static LoggerAsyncMode LogReceiverMode { get; set; } = LoggerAsyncMode.Continuous;

        /// <summary>
        /// Gets/sets the time between two warnings.
        /// </summary>
        public static TimeSpan TimeBetweenWarnings { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Gets/sets the time in milli seconds for detecting late messages. Messages older than this value will result in a warning message to the log system.
        /// </summary>
        public static int LateMessageMilliSeconds { get; set; } = 1000;

        /// <summary>
        /// Gets / sets the maximum number of messages allowed to be older than <see cref="LateMessageMilliSeconds"/>
        /// </summary>
        public static int LateMessageTreshold { get; set; } = 1000;

        /// <summary>
        /// Retrieves whether the async logger thread is currently idle or not
        /// </summary>
        public static bool Idle { get; private set; }

        static Queue<LoggingEvent> queue = new Queue<LoggingEvent>();
        static object syncRoot = new object();
        static long currentDelayMilliSeconds;

        static LoggerAsync()
        {
            new Thread(Worker).Start();
        }

        static void Worker()
        {
            Thread.CurrentThread.Name = "LoggerAsync";
            Thread.CurrentThread.IsBackground = true;
#if !NETCOREAPP10
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
#endif
            ILog log = LogManager.GetLogger(typeof(LoggerAsync));
            log.Debug("Worker started.");
            bool delayWarningSent = false;
            try
            {
                DateTime nextWarningUtc = DateTime.MinValue;
                int discardedCount = 0;

                while (true)
                {
                    Queue<LoggingEvent> events = null;
                    //wait for messages
                    lock (syncRoot)
                    {
                        while (true)
                        {
                            if (queue.Count > 0)
                            {
                                events = queue;
                                queue = new Queue<LoggingEvent>();
                                break;
                            }
                            //entering idle mode
                            if (delayWarningSent)
                            {
                                log.Warn("Backlog recovered.");
                                delayWarningSent = false;
                                continue;
                            }
                            Idle = true;
                            //wait for pulse
                            while (true)
                            {
                                Monitor.Wait(syncRoot, 1000);
                                break;
                            }
                            Idle = false;
                        }
                    }

                    foreach (LoggingEvent e in events)
                    {
                        long delayTicks = (DateTime.UtcNow - e.TimeStampUtc).Ticks;
                        currentDelayMilliSeconds = (int)(delayTicks / TimeSpan.TicksPerMillisecond);

                        //do we have late messages ?
                        if (currentDelayMilliSeconds > LateMessageMilliSeconds)
                        {
                            //yes, opportune logging ?
                            if (LogReceiverMode == LoggerAsyncMode.Opportune)
                            {
                                //discard old notifications
                                if (delayTicks / TimeSpan.TicksPerMillisecond > LateMessageMilliSeconds)
                                {
                                    discardedCount++;
                                    continue;
                                }
                            }
                            else
                            {
                                //no continous logging -> warn user
                                if ((events.Count > LateMessageTreshold) && (DateTime.UtcNow > nextWarningUtc))
                                {
                                    log.WarnFormat("Async logging has a backlog of {0} events, current delay is {1}!", events.Count, currentDelayMilliSeconds.FormatTimeShort());
                                    //calc next
                                    nextWarningUtc = DateTime.UtcNow + TimeBetweenWarnings;
                                    delayWarningSent = true;
                                }
                            }
                        }
                        log.Logger.Log(e);
                    }
                    if (discardedCount > 0)
                    {
                        if (DateTime.UtcNow > nextWarningUtc)
                        {
                            log.WarnFormat("Async logging has discarded {0} events, current delay is {1}!", discardedCount, currentDelayMilliSeconds.FormatTimeShort());
                            discardedCount = 0;
                            nextWarningUtc = DateTime.UtcNow + TimeBetweenWarnings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Fatal("Fatal exception in async logger, no more async logging possible!", ex);
            }
        }

        ILogger logger;

        #region Public Instance Constructors

        /// <summary>
        /// Construct a new wrapper for the specified <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance to wrap.</param>
        /// <remarks>
        /// <para>
        /// Construct a new wrapper for the specified <see cref="ILogger"/> instance.
        /// </para>
        /// </remarks>
        public LoggerAsync(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion Public Instance Constructors

        #region Implementation of ILogger

        /// <summary>
        /// Gets the logger name.
        /// </summary>
        /// <value>
        /// The name of the logger.
        /// </value>
        /// <remarks>
        /// <para>
        /// The name of this logger
        /// </para>
        /// </remarks>
        public string Name => logger.Name;

        /// <summary>
        /// Gets the <see cref="ILoggerRepository"/> where this 
        /// <c>Logger</c> instance is attached to.
        /// </summary>
        /// <value>
        /// The <see cref="ILoggerRepository" /> that this logger belongs to.
        /// </value>
        /// <remarks>
        /// <para>
        /// Gets the <see cref="ILoggerRepository"/> where this 
        /// <c>Logger</c> instance is attached to.
        /// </para>
        /// </remarks>
        public ILoggerRepository Repository => logger.Repository;

        /// <summary>
        /// Checks if this logger is enabled for a given <see cref="Level"/> passed as parameter.
        /// </summary>
        /// <param name="level">The level to check.</param>
        /// <returns>
        /// <c>true</c> if this logger is enabled for <c>level</c>, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Test if this logger is going to log events of the specified <paramref name="level"/>.
        /// </para>
        /// <para>
        /// This method must not throw any exception to the caller.
        /// </para>
        /// </remarks>
        public bool IsEnabledFor(Level level) => logger.IsEnabledFor(level);

        /// <summary>
        /// Creates a new logging event and logs the event without further checks.
        /// </summary>
        /// <param name="callerStackBoundaryDeclaringType">The declaring type of the method that is
        /// the stack boundary into the logging system for this call.</param>
        /// <param name="level">The level of the message to be logged.</param>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        /// <remarks>
        /// <para>
        /// Generates a logging event and delivers it to the attached
        /// appenders.
        /// </para>
        /// </remarks>
        public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception)
        {
            Log(new LoggingEvent(callerStackBoundaryDeclaringType, Repository, Name, level, message, exception));
        }

        /// <summary>
        /// Creates a new logging event and logs the event without further checks.
        /// </summary>
        /// <param name="logEvent">The event being logged.</param>
        /// <remarks>
        /// <para>
        /// Delivers the logging event to the attached appenders.
        /// </para>
        /// </remarks>
        public void Log(LoggingEvent logEvent)
        {
            if (logEvent == null)
            {
                return;
            }

            lock (queue)
            {
                queue.Enqueue(logEvent);
            }
        }

        #endregion
    }
}