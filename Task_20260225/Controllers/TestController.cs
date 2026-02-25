using Microsoft.AspNetCore.Mvc;

namespace Task_20260225.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        await Task.Delay(500);
        return Ok("112");
    }
}