using ManualMovements.Domain.Constants;
using System;

namespace ManualMovements.Domain.Entities.Response
{
    public abstract class PagedResponse<TData>: BaseResponse<TData>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);

        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        public PagedResponse(
            TData data,
            int page,
            int pageSize,
            int total,
            int code = 200,
            string? message = null
            ) : base(data, code, message)
        {
            Page = page;
            PageSize = pageSize;
            Total = total;
        }

        public PagedResponse(
            TData? data,
            int totalCount,
            int page = Configuration.DEFAULT_PAGE,
            int pageSize = Configuration.PAGE_SIZE
            )
            : base(data)
        {
            Data = data;
            Page = page;
            PageSize = pageSize;
            Total = totalCount;
        }

        public PagedResponse() { }
    }
}
