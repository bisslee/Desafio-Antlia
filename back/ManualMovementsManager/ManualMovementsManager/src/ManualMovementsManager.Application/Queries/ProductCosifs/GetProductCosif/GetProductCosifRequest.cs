using MediatR;
using System;

namespace ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosif
{
    public class GetProductCosifRequest : BaseRequest, IRequest<GetProductCosifResponse>
    {
        public string? ProductCode { get; set; }
        public string? CosifCode { get; set; }
        public string? ClassificationCode { get; set; }
        public bool? Active { get; set; }
    }
} 