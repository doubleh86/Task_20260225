namespace Task_20260225.Queries;

public class GetEmployeeByNameQuery : IQueryBase
{
    public readonly string Name;

    public GetEmployeeByNameQuery(string name)
    {
        Name = name;
    }
}
