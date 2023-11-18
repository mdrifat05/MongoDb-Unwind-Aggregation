namespace Domain.Params;

public class GenericParams
{
    private const int MaxPageSize = 100;

    private int _pageSize = 10;
    private string? _search;
    public int PageIndex { get; set; } = 0;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    // public string? Sort { get; set; }
    public int OrderBy { get; set; } = 0;

    public string? Search
    {
        get => _search;
        set => _search = value?.ToLower();
    }
}