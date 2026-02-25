using Task_20260225.Command;

namespace Task_20260225.Queries;

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