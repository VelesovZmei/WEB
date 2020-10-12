using System;
using Microsoft.Extensions.Logging;

namespace WEB.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _path;
        private bool _disposed = false;
        private ILogger _logger;

        public FileLoggerProvider(string path)
        {
            _path = path;
        }
        public ILogger CreateLogger(string category)
        {
            if (_logger == null)
            {
                _logger = new FileLogger(_path);
            }
            return _logger;
        }

        /// <summary>
        /// Throws if this class has been disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release the unmanaged resources used by the provider and optionally release the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed objects
                    if (_logger != null)
                    {
                        _logger = null;
                    }
                }
                // Release unmanaged resources
                _disposed = true;
            }
        }

        ~FileLoggerProvider()
        {
            Dispose(false);
        }
    }
}
