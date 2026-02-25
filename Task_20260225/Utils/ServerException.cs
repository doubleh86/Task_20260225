namespace Task_20260225.Utils;

public class ServerException : Exception
{
    public override string Message { get; }
    public readonly int ResultCode;

    public ServerException(int resultCode, string message)
    {
        Message = message;
        ResultCode = resultCode;
    }
}