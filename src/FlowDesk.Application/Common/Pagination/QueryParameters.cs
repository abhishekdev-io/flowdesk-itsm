namespace FlowDesk.Application.Common.Pagination;

public record QueryParameters
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public bool Descending { get; init; } = false;
}
