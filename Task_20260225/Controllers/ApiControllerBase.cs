using Microsoft.AspNetCore.Mvc;
using Task_20260225.Common.Services;
using Task_20260225.Common.Utils;

namespace Task_20260225.Controllers;

public abstract class ApiControllerBase : ControllerBase, IDisposable
{
    protected readonly ContactCacheService _cacheService;
    protected readonly TaskServerServices _serverService;

    protected ApiControllerBase(ContactCacheService cacheService, TaskServerServices serverServices)
    {
        _cacheService = cacheService;
        _serverService = serverServices;
    }
    
    protected ActionResult _HandleServerException(ServerException exception)
    {
        _serverService.LoggerService.Warning(exception.Message, exception);
        var statusCode = _GetStatusCode(exception.ResultCode);
        var response = new
        {
            ResultCode = (int)exception.ResultCode,
            exception.Message
        };

        return StatusCode(statusCode, response);
    }

    protected ActionResult _HandleUnknownException(Exception exception)
    {
        _serverService.LoggerService.Error(exception.Message, exception);
        return StatusCode(500, new
        {
            ResultCode = (int)ErrorCode.InvalidRequest,
            Message = "Internal server error."
        });
    }

    private static int _GetStatusCode(ErrorCode errorCode)
    {
        return errorCode switch
        {
            ErrorCode.UploadEmployeeInfoWrongFileType => StatusCodes.Status415UnsupportedMediaType,
            _ => StatusCodes.Status400BadRequest
        };
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
    }
}