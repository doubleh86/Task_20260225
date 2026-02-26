namespace Task_20260225.Common.Utils;

public class ServerException : Exception
{
    public override string Message { get; }
    public readonly ErrorCode ResultCode;

    public ServerException(ErrorCode resultCode, string message)
    {
        Message = message;
        ResultCode = resultCode;
    }
}