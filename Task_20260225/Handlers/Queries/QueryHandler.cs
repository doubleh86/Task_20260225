using Task_20260225.Queries;
using Task_20260225.Services;

namespace Task_20260225.Handlers.Queries;

public abstract class QueryHandler<T> : IHandlerBase<T>, IDisposable
{
    protected readonly IQueryBase _query;
    protected readonly ContactCacheService _cacheService;
    public abstract Task<T> HandleAsync();

    protected QueryHandler(IQueryBase query, ContactCacheService cacheService)
    {
        _query = query;
        _cacheService = cacheService;
    }
    
    protected abstract void _Dispose();

    public void Dispose()
    {
        _Dispose();
    }
}