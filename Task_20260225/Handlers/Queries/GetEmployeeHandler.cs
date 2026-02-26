using System.Text.Json;
using Task_20260225.Application.Queries;
using Task_20260225.Common.Services;
using Task_20260225.Common.Utils;

namespace Task_20260225.Handlers.Queries;

public class GetEmployeeHandler : QueryHandler<string>
{
    public GetEmployeeHandler(GetEmployeeQuery query, ContactCacheService cacheService, LoggerService loggerService) 
        : base(query, cacheService, loggerService)
    {
    }

    public override Task<string> HandleAsync()
    {
        if (_query is not GetEmployeeQuery query)
        {
            throw new ServerException(ErrorCode.InvalidQuery, "Invalid Query [GetEmployeeQuery]");
        }
        
        if(query.Page < 1 || query.PageSize < 1)
            throw new ServerException(ErrorCode.GetEmployeeWrongPageOrPageSize, 
                $"Request wrong Page or Page size [GetEmployeeQuery][Page : {query.Page}, Page : {query.PageSize}]");

        var totalCount = _cacheService.GetContactCount();
        var totalPages = query.PageSize > 0
            ? (int)Math.Ceiling(totalCount / (double)query.PageSize)
            : 0;
        var contacts = _cacheService.GetContactList(query.Page, query.PageSize);
        var response = new
        {
            totalCount,
            totalPages,
            items = contacts
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return Task.FromResult(json);
    }

    protected override void _Dispose()
    {
    }
}
