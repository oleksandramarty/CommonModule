using CommonModule.Shared.Common;

namespace CommonModule.Shared.Requests.Base;

public class PaginatorRequest: PaginatorEntity
{
    public PaginatorRequest(int pageNumber, int pageSize, bool isFull) : base(pageNumber, pageSize, isFull)
    {
    }
}