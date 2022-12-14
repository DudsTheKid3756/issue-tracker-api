namespace IssueTracker.Logging;

public class FileLogger : ILogger
{
    private static readonly object Lock = new();
    private readonly string _filePath;

    public FileLogger(string path)
    {
        _filePath = path;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null!;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == LogLevel.Trace;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        lock (Lock)
        {
            var fullFilePath = Path.Combine(_filePath, DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt");
            var n = Environment.NewLine;
            var exc = "";
            if (exception != null) exc = exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
            File.AppendAllText(fullFilePath,
                logLevel + ": " + DateTime.Now + " " + formatter(state, exception!) + n + exc);
        }
    }
}