using Task_20260225.Application.Queries;
using Task_20260225.Common.Services;

namespace Task_20260225.Handlers.Queries;

public abstract class QueryHandler<T> : IHandlerBase<T>, IDisposable
{
    protected readonly IQueryBase _query;
    protected readonly ContactCacheService _cacheService;
    protected readonly LoggerService _loggerService;
    public abstract Task<T> HandleAsync();

    protected QueryHandler(IQueryBase query, ContactCacheService cacheService, LoggerService? loggerService)
    {
        _query = query;
        _cacheService = cacheService;
        _loggerService = loggerService;
    }
    
    protected abstract void _Dispose();

    public void Dispose()
    {
        _Dispose();
    }
}