using System.Text.Json;
using Task_20260225.Common.Services;
using Task_20260225.Queries;

namespace Task_20260225.Handlers.Queries;

public class GetEmployeeByNameHandler : QueryHandler<string>
{
    public GetEmployeeByNameHandler(GetEmployeeByNameQuery query, ContactCacheService cacheService)
        : base(query, cacheService)
    {
    }

    public override Task<string> HandleAsync()
    {
        var query = _query as GetEmployeeByNameQuery;
        ArgumentNullException.ThrowIfNull(query);

        var contacts = _cacheService.GetContactListByName(query.Name);
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
