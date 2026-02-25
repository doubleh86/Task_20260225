using Microsoft.AspNetCore.Mvc;

namespace Task_20260225.Controllers;

public class ApiControllerBase : ControllerBase, IDisposable
{
    public void Dispose()
    {
        // TODO release managed resources here
    }
}