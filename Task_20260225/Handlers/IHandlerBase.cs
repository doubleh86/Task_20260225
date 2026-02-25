namespace Task_20260225.Handlers;

public interface IHandlerBase<T>
{
    Task<T> HandleAsync();
}