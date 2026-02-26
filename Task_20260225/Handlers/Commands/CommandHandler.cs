using Task_20260225.Application.Command;
using Task_20260225.Common.Services;
using Task_20260225.Handlers;

namespace Task_20260225.Application.Handlers.Commands;

public abstract class CommandHandler<T> : IHandlerBase<T>, IDisposable
{
    protected readonly ICommandBase _command; 
    protected readonly ContactCacheService _cacheService;
    protected readonly LoggerService _loggerService;
    public abstract Task<T> HandleAsync();
    protected abstract void _Dispose();

    protected CommandHandler(ICommandBase command, ContactCacheService cacheService, LoggerService? loggerService)
    {
        _command = command;
        _cacheService = cacheService;
        _loggerService = loggerService;
    }
    
    public void Dispose()
    {
        _Dispose();
    }
}