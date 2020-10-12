using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace WEB.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        private object _lock = new object();

        public FileLogger(string path)
        {
            _filePath = path;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel level)
        {
            return true;
        }

        public void Log<TState>(LogLevel level, EventId id, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            if (formatter != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(_filePath,
                        level.ToString() + " : " + DateTime.UtcNow.ToString("o") + Environment.NewLine +
                        "\t" + formatter(state, exception) + Environment.NewLine);
                }
            }
        }
    }
}
