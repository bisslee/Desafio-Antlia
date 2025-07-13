using MediatR;
using System;

namespace ManualMovementsManager.Application.Queries.Products.GetProduct
{
    public class GetProductRequest : BaseRequest, IRequest<GetProductResponse>
    {
        public string? ProductCode { get; set; }
        public string? Description { get; set; }
        public bool? Active { get; set; }
    }
} 