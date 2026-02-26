namespace Task_20260225.Application.Queries;

public class GetEmployeeQuery : IQueryBase
{
    public readonly int Page;
    public readonly int PageSize;

    public GetEmployeeQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}