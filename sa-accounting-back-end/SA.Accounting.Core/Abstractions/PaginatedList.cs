namespace SA.Accounting.Core.Abstractions;
public class PaginatedList<T>
{
    public List<T> Items { get; private set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    private PaginatedList(List<T> items, int count , int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public static PaginatedList<T> Create(List<T> items, int count, int pageNumber, int pageSize)
    {
        return new PaginatedList<T>(items, count, pageNumber,  pageSize);
    }
}


