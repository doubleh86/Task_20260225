using System.Text.Json;
using Task_20260225.Queries;
using Task_20260225.Services;

namespace Task_20260225.Handlers.Queries;

public class GetEmployeeHandler : QueryHandler<string>
{
    public GetEmployeeHandler(GetEmployeeQuery query, ContactCacheService cacheService) : base(query, cacheService)
    {
    }

    public override Task<string> HandleAsync()
    {
        var query = _query as GetEmployeeQuery;
        ArgumentNullException.ThrowIfNull(query);

        var contacts = _cacheService.GetContactList(query.Page, query.PageSize);
        var json = JsonSerializer.Serialize(contacts, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return Task.FromResult(json);
    }

    protected override void _Dispose()
    {
    }
}
