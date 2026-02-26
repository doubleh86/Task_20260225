using Microsoft.AspNetCore.Mvc;
using Task_20260225.Application.Command;
using Task_20260225.Application.Queries;
using Task_20260225.Common.Services;
using Task_20260225.Common.Utils;
using Task_20260225.Handlers.Commands;
using Task_20260225.Handlers.Queries;

namespace Task_20260225.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ApiControllerBase
{
    

    public EmployeeController(ContactCacheService cacheService, TaskServerServices serverServices) 
        : base(cacheService, serverServices)
    {
    }

    [HttpGet]
    public async Task<ActionResult> Get(int page, int pageSize)
    {
        try
        {
            var query = new GetEmployeeQuery(page, pageSize);
            using var handler = new GetEmployeeHandler(query, _cacheService, _serverService.LoggerService);

            var result = await handler.HandleAsync();
            return Content(result, "application/json");
        }
        catch (ServerException e)
        {
            return _HandleServerException(e);
        }
        catch (Exception e)
        {
            return _HandleUnknownException(e);
        }
    }

    [HttpGet("{name}")]
    public async Task<ActionResult> Get(string name)
    {
        try
        {
            var query = new GetEmployeeByNameQuery(name);
            using var handler = new GetEmployeeByNameHandler(query, _cacheService, _serverService.LoggerService);

            var result = await handler.HandleAsync();
            return Content(result, "application/json");
        }
        catch (ServerException e)
        {
            return _HandleServerException(e);
        }
        catch (Exception e)
        {
            return _HandleUnknownException(e);
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> Post([FromForm] IFormFile? file, [FromForm] string? text)
    {
        try
        {
            if ((file is null || file.Length == 0) && string.IsNullOrWhiteSpace(text))
                return BadRequest("file or text is required.");

            var command = file is not null && file.Length > 0
                ? new UploadEmployeeInfoCommand(file)
                : new UploadEmployeeInfoCommand(text!);

            using var handler = new UploadEmployeeInfoHandler(command, _cacheService, _serverService.LoggerService);
            var (successCount, failCount) = await handler.HandleAsync();

            return StatusCode(201, new
            {
                SuccessCount = successCount,
                FailedCount = failCount
            });
        }
        catch (ServerException e)
        {
            return _HandleServerException(e);
        }
        catch (Exception e)
        {
            return _HandleUnknownException(e);
        }
    }

    
}
