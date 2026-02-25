using Microsoft.AspNetCore.Mvc;
using Task_20260225.Command;
using Task_20260225.Handlers.Commands;
using Task_20260225.Handlers.Queries;
using Task_20260225.Queries;
using Task_20260225.Services;

namespace Task_20260225.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ApiControllerBase
{
    private readonly ContactCacheService _cacheService;

    public EmployeeController(ContactCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<ActionResult> Get(int page, int pageSize)
    {
        var query = new GetEmployeeQuery(page, pageSize);
        using var handler = new GetEmployeeHandler(query, _cacheService);

        var result = await handler.HandleAsync();
        return Content(result, "application/json");
    }

    [HttpGet("{name}")]
    public async Task<ActionResult> Get(string name)
    {
        var query = new GetEmployeeByNameQuery(name);
        using var handler = new GetEmployeeByNameHandler(query, _cacheService);

        var result = await handler.HandleAsync();
        return Content(result, "application/json");
    }

    [HttpPost]
    public async Task<ActionResult<int>> Post([FromForm] IFormFile? file, [FromForm] string? text)
    {
        if ((file is null || file.Length == 0) && string.IsNullOrWhiteSpace(text))
            return BadRequest("file or text is required.");

        var command = file is not null && file.Length > 0
            ? new UploadEmployeeInfoCommand(file)
            : new UploadEmployeeInfoCommand(text!);

        using var handler = new UploadEmployeeInfoHandler(command, _cacheService);
        var count = await handler.HandleAsync();
        return Ok(count);
    }
}
