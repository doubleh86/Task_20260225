using System.Runtime.CompilerServices;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Task_20260225.Common.Services;

public class LoggerService
{
    private ILogger _logger;
    
    public void CreateLogger(IConfiguration configuration)
    {
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.ReadFrom.Configuration(configuration);
        
        _logger = loggerConfiguration.CreateLogger();
    }

    public void Error(string message, Exception exception = null)
    {
        if (exception != null)
        {
            _logger?.Error(exception, message);
        }
        else
        {
            _logger?.Error(message);    
        }
    }

    public void Error(object source, string message, Exception exception = null, [CallerMemberName] string methodName = "")
    {
        var className = source.GetType().Name;
        var logger = _logger?.ForContext("SourceContext", className)
                             .ForContext("MethodName", methodName);
        if (exception != null)
        {
            logger?.Error(exception, message);
        }
        else
        {
            logger?.Error(message);
        }
    }

    public void Information(string message)
    {
        _logger?.Information(message);
    }
    
    public void Information(object source, string message, [CallerMemberName] string methodName = "")
    {
        var className = source.GetType().Name;
        _logger?.ForContext("SourceContext", className)
                .ForContext("MethodName", methodName)
                .Information(message);
    }

    public void Warning(string message, Exception exception = null)
    {
        if (exception != null)
        {
            _logger?.Warning(exception, message);
        }
        else
        {
            _logger?.Warning(message);
        }
    }

    public void Warning(object source, string message, Exception exception = null, [CallerMemberName] string methodName = "")
    {
        var className = source.GetType().Name;
        var logger = _logger?.ForContext("SourceContext", className)
                             .ForContext("MethodName", methodName);
        if (exception != null)
        {
            logger?.Warning(exception, message);
        }
        else
        {
            logger?.Warning(message);
        }
    }
    
    public void Debug(string message)
    {
        _logger?.Debug(message);
    }
}
