using System.Text.Json;
using Task_20260225.Application.Queries;
using Task_20260225.Common.Services;
using Task_20260225.Common.Utils;

namespace Task_20260225.Handlers.Queries;

public class GetEmployeeByNameHandler : QueryHandler<string>
{
    public GetEmployeeByNameHandler(GetEmployeeByNameQuery query, ContactCacheService cacheService, LoggerService loggerService)
        : base(query, cacheService, loggerService)
    {
    }

    public override Task<string> HandleAsync()
    {
        if (_query is not GetEmployeeByNameQuery query)
        {
            throw new ServerException(ErrorCode.InvalidQuery, "Invalid Query [GetEmployeeByNameQuery]");
        }

        if (string.IsNullOrWhiteSpace(query.Name) == true)
        {
            throw new ServerException(ErrorCode.GetEmployeeByNameEmptyName, 
                "Request Name Field is Empty [GetEmployeeByNameQuery]");
        }
            
        var contacts = _cacheService.GetContactListByName(query.Name);
        if (contacts.Count == 0)
        {
            _loggerService?.Information(this, "No employee found by name.");
        }
        
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
