using Task_20260225.Command;
using Task_20260225.Common.Services;

namespace Task_20260225.Handlers.Commands;

public abstract class CommandHandler<T> : IHandlerBase<T>, IDisposable
{
    protected readonly ICommandBase _command; 
    protected readonly ContactCacheService _cacheService;
    public abstract Task<T> HandleAsync();
    protected abstract void _Dispose();

    protected CommandHandler(ICommandBase command, ContactCacheService cacheService)
    {
        _command = command;
        _cacheService = cacheService;
    }
    
    public void Dispose()
    {
        _Dispose();
    }
}