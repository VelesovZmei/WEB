using Microsoft.Extensions.Logging;

namespace WEB.Logging
{
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string path)
        {
            return builder.AddProvider(new FileLoggerProvider(path));
        }
    }
}
