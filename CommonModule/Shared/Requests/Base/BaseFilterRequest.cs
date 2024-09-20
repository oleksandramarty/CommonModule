namespace CommonModule.Shared.Requests.Base;

public class BaseFilterRequest<TId>
{
    public PaginatorRequest? Paginator { get; set; }
    public BaseSortableRequest? Sort { get; set; }
    public BaseDateRangeFilterRequest? DateRange { get; set; }
    
    public string? Query { get; set; }
    
    public IEnumerable<TId>? Ids { get; set; }
}