namespace MoneyLoris.Application.Common;
public class Pagination<T>
{
    public Pagination(T dataPage, long total)
    {
        DataPage = dataPage;
        Total = total;
    }

    public T DataPage { get; private set; } = default!;
    public long Total { get; private set; }
}

public class PaginationFilter
{
    public int CurrentPage { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 25;
}
