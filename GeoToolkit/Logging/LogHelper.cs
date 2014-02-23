using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using log4net.Core;
using NUnit.Framework;

namespace GeoToolkit.Logging
{
	public static class LogHelper
	{
		private static readonly Dictionary<Type, GeoLogger> _loggers = new Dictionary<Type, GeoLogger>();
        private static readonly object _syncRoot = new object();

		public static event EventHandler<LogEventArgs> Logging;

	    private static void Log(object obj, Level level, object message, Exception exception)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var logger = GetLogger(obj.GetType(),
                                   s =>
                                       {
                                           var stackFrames = new StackTrace().GetFrames();

                                           if (stackFrames == null)
                                               return s;

                                           if (stackFrames.Length < 6)
                                               return s;

                                           var stackFrame = stackFrames[5];
                                           var method = (MethodInfo)stackFrame.GetMethod();

                                           return string.Format("[{0}.{1}] {2}",
                                                                obj.GetType().Name,
                                                                method.Name, s);
                                       });

	        if (level == Level.Info)
                logger.Info(message, exception);
            else if (level == Level.Debug)
                logger.Debug(message, exception);
            else if (level == Level.Warn)
                logger.Warn(message, exception);
            else if (level == Level.Error)
                logger.Error(message, exception);
            else if (level == Level.Fatal)
                logger.Fatal(message, exception);
        }

        public static void LogInfo(this object obj, object message, Exception exception)
        {
            Log(obj, Level.Info, message, exception);
        }

        [StringFormatMethod("format")]
        public static void LogInfoFormat(this object obj, string format, params object[] objs)
        {
            Log(obj, Level.Info, string.Format(format, objs), null);
        }

        public static void LogInfo(this object obj, object message)
        {
            Log(obj, Level.Info, message, null);
        }

        public static void LogDebug(this object obj, object message, Exception exception)
        {
            Log(obj, Level.Debug, message, exception);
        }

        [StringFormatMethod("format")]
        public static void LogDebugFormat(this object obj, string format, params object[] objs)
        {
            Log(obj, Level.Debug, string.Format(format, objs), null);
        }

        public static void LogDebug(this object obj, object message)
        {
            Log(obj, Level.Debug, message, null);
        }

        public static void LogWarn(this object obj, object message, Exception exception)
        {
            Log(obj, Level.Warn, message, exception);
        }

        [StringFormatMethod("format")]
        public static void LogWarnFormat(this object obj, string format, params object[] objs)
        {
            Log(obj, Level.Warn, string.Format(format, objs), null);
        }

        public static void LogWarn(this object obj, object message)
        {
            Log(obj, Level.Warn, message, null);
        }

        public static void LogError(this object obj, object message, Exception exception)
        {
            Log(obj, Level.Error, message, exception);
        }

        [StringFormatMethod("format")]
        public static void LogErrorFormat(this object obj, string format, params object[] objs)
        {
            Log(obj, Level.Error, string.Format(format, objs), null);
        }

        public static void LogError(this object obj, object message)
        {
            Log(obj, Level.Error, message, null);
        }

        public static void LogFatal(this object obj, object message, Exception exception)
        {
            Log(obj, Level.Fatal, message, exception);
        }

        [StringFormatMethod("format")]
        public static void LogFatalFormat(this object obj, string format, params object[] objs)
        {
            Log(obj, Level.Fatal, string.Format(format, objs), null);
        }

        public static void LogFatal(this object obj, object message)
        {
            Log(obj, Level.Fatal, message, null);
        }

		public static ILog GetLogger(Type type, Func<string, string> strConverter = null)
		{
		    lock (_syncRoot)
		    {
		        GeoLogger result;
		        if (_loggers.TryGetValue(type, out result))
		            return result;

		        var logger = new GeoLogger(type, strConverter);
		        logger.LogEventRaised += LoggerLogEventRaised;
			
		        _loggers.Add(type, logger);

		        return logger;
		    }
		}

		private static void LoggerLogEventRaised(object sender, LogEventArgs e)
		{
		    var handler = Logging;

		    if (handler == null) 
                return;

		    e.Message = string.Format("{0}, {1}: {2}{3}",
		                              DateTime.Now.ToString("dd.MM hh:mm:ss"), e.Level, e.Message,
                                      e.Exception != null ? Environment.NewLine + e.Exception : string.Empty);

		    handler(null, e);
		}

		private class GeoLogger : ILog
		{
			private readonly ILog _logger;
		    private readonly Func<string, string> _strConverter;

		    public Type Type { get; private set; }

		    public event EventHandler<LogEventArgs> LogEventRaised;

			private void OnLogEventRaised(LogEventArgs eventArgs)
			{
			    var handler = LogEventRaised;
                if (handler != null)
                    handler(this, eventArgs);
			}

			public GeoLogger(Type type, Func<string, string> strConverter = null)
			{
				_logger = LogManager.GetLogger(type);
			    _strConverter = strConverter;
			    Type = type;
			}

            private string Convert(string str)
            {
                if (_strConverter == null)
                    return str;

                return _strConverter(str);
            }

		    public void Debug(object message)
			{
		        var msg = Convert(message.ToString());

                _logger.Debug(msg);

		        OnLogEventRaised(new LogEventArgs(Level.Debug, msg));
			}

			public void Debug(object message, Exception exception)
			{
			    var str = Convert(message.ToString());

                _logger.Debug(str, exception);

			    OnLogEventRaised(new LogEventArgs(Level.Debug, str, exception));
			}

            [StringFormatMethod("format")]
			public void DebugFormat(string format, params object[] args)
			{
			    var str = Convert(string.Format(format, args));

                _logger.DebugFormat(str);

			    OnLogEventRaised(new LogEventArgs(Level.Debug, str));
			}

			public void DebugFormat(string format, object arg0)
			{
				DebugFormat(format, new[] { arg0 });
			}

			public void DebugFormat(string format, object arg0, object arg1)
			{
				DebugFormat(format, new[] { arg0, arg1 });
			}

			public void DebugFormat(string format, object arg0, object arg1, object arg2)
			{
				DebugFormat(format, new[] { arg0, arg1, arg2 });
			}

			public void DebugFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void Info(object message)
			{
			    var str = Convert(message.ToString());

                _logger.Info(str);

			    OnLogEventRaised(new LogEventArgs(Level.Info, str));
			}

			public void Info(object message, Exception exception)
			{
			    var str = Convert(message.ToString());

                _logger.Info(str, exception);

			    OnLogEventRaised(new LogEventArgs(Level.Info, str, exception));
			}

            [StringFormatMethod("format")]
			public void InfoFormat(string format, params object[] args)
			{
			    var str = Convert(string.Format(format, args));

                _logger.InfoFormat(str);

			    OnLogEventRaised(new LogEventArgs(Level.Info, str));
			}

			public void InfoFormat(string format, object arg0)
			{
				InfoFormat(format, new[] { arg0 });
			}

			public void InfoFormat(string format, object arg0, object arg1)
			{
				InfoFormat(format, new[] { arg0, arg1 });
			}

			public void InfoFormat(string format, object arg0, object arg1, object arg2)
			{
				InfoFormat(format, new[] { arg0, arg1, arg2 });
			}

			public void InfoFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void Warn(object message)
			{
			    var str = Convert(message.ToString());

                _logger.Warn(str);

			    OnLogEventRaised(new LogEventArgs(Level.Warn, str));
			}

			public void Warn(object message, Exception exception)
			{
			    var str = Convert(message.ToString());

                _logger.Warn(str, exception);

			    OnLogEventRaised(new LogEventArgs(Level.Warn, str, exception));
			}

            [StringFormatMethod("format")]
			public void WarnFormat(string format, params object[] args)
			{
			    var str = Convert(string.Format(format, args));

                _logger.WarnFormat(str);

			    OnLogEventRaised(new LogEventArgs(Level.Warn, str));
			}

			public void WarnFormat(string format, object arg0)
			{
				WarnFormat(format, new[] { arg0 });
			}

			public void WarnFormat(string format, object arg0, object arg1)
			{
				WarnFormat(format, new[] { arg0, arg1 });
			}

			public void WarnFormat(string format, object arg0, object arg1, object arg2)
			{
				WarnFormat(format, new[] { arg0, arg1, arg2 });
			}

			public void WarnFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void Error(object message)
			{
			    var str = Convert(message.ToString());

                _logger.Error(str);

			    OnLogEventRaised(new LogEventArgs(Level.Error, str));
			}

			public void Error(object message, Exception exception)
			{
			    var str = Convert(message.ToString());

                _logger.Error(str, exception);

			    OnLogEventRaised(new LogEventArgs(Level.Error, str, exception));
			}

            [StringFormatMethod("format")]
			public void ErrorFormat(string format, params object[] args)
			{
			    var str = Convert(string.Format(format, args));

                _logger.ErrorFormat(str);

			    OnLogEventRaised(new LogEventArgs(Level.Error, str));
			}

			public void ErrorFormat(string format, object arg0)
			{
				ErrorFormat(format, new[] { arg0 });
			}

			public void ErrorFormat(string format, object arg0, object arg1)
			{
				ErrorFormat(format, new[] { arg0, arg1 });
			}

			public void ErrorFormat(string format, object arg0, object arg1, object arg2)
			{
				ErrorFormat(format, new[] { arg0, arg1, arg2 });
			}

			public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void Fatal(object message)
			{
			    var str = Convert(message.ToString());

                _logger.Fatal(str);

			    OnLogEventRaised(new LogEventArgs(Level.Fatal, str));
			}

			public void Fatal(object message, Exception exception)
			{
			    var str = Convert(message.ToString());

                _logger.Fatal(str, exception);

			    OnLogEventRaised(new LogEventArgs(Level.Fatal, str, exception));
			}

            [StringFormatMethod("format")]
			public void FatalFormat(string format, params object[] args)
			{
			    var str = Convert(string.Format(format, args));

                _logger.FatalFormat(str);

			    OnLogEventRaised(new LogEventArgs(Level.Fatal, str));
			}

			public void FatalFormat(string format, object arg0)
			{
				FatalFormat(format, new[] { arg0 });
			}

			public void FatalFormat(string format, object arg0, object arg1)
			{
				FatalFormat(format, new[] { arg0, arg1 });
			}

			public void FatalFormat(string format, object arg0, object arg1, object arg2)
			{
				FatalFormat(format, new[] { arg0, arg1, arg2 });
			}

			public void FatalFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public bool IsDebugEnabled
			{
				get { return _logger.IsDebugEnabled; }
			}

			public bool IsInfoEnabled
			{
				get { return _logger.IsInfoEnabled; }
			}

			public bool IsWarnEnabled
			{
				get { return _logger.IsWarnEnabled; }
			}

			public bool IsErrorEnabled
			{
				get { return _logger.IsErrorEnabled; }
			}

			public bool IsFatalEnabled
			{
				get { return _logger.IsFatalEnabled; }
			}

			public ILogger Logger
			{
				get { return _logger.Logger; }
			}
		}

		public class LogEventArgs : EventArgs
		{
		    public string Message { get; set; }

		    public Exception Exception { get; private set; }

		    public Level Level { get; private set; }

		    public LogEventArgs(Level level, string message, Exception exception)
			{
				Message = message;
				Exception = exception;
				Level = level;
			}

			public LogEventArgs(Level level, object message, Exception exception)
				: this(level, message.ToString(), exception)
			{
			}

			public LogEventArgs(Level level, string message) : this(level, message, null)
			{
			}

			public LogEventArgs(Level level, object message) : this(level, message.ToString())
			{
			}
		}
	}

#if DEBUG

    [TestFixture]
    public class LogHelperTest
    {
        [Test]
        public void LogTest()
        {
            this.LogInfo("test");
        }

        [Test]
        public void LoadTest()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < 100000; i++)
                this.LogDebug("test string");

            stopWatch.Stop();

            Console.WriteLine("time for 100000 log items: {0}ms", stopWatch.ElapsedMilliseconds);
        }
    }

#endif
}