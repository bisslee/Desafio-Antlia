namespace ManualMovementsManager.Application.Queries
{
    public class BaseRequest
    {
        /// <summary>
        /// Total Itens per page
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Actual ordered page
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Field name to order
        /// 
        public string? FieldName { get; set; }

        /// <summary>
        /// order type
        /// 
        public string? Order { get; set; } = "asc";

    }

    public static class RequestExtensions
    {
        public static T LoadPagination<T>(this T request) where T : BaseRequest
        {
            request.Page = GetPage(request.Page);
            request.Offset = GetPageSize(request.Offset);
            return request;
        }
        public static int GetPage(int? page)
        {
            return page.HasValue && page.Value > 0 ? page.Value : 1;
        }
        public static int GetPageSize(int? pageSize)
        {
            return pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;
        }
    }

}
