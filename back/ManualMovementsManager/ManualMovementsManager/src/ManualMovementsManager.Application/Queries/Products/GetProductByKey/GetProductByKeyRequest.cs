using MediatR;
using System;

namespace ManualMovementsManager.Application.Queries.Products.GetProductByKey
{
    public class GetProductByKeyRequest : IRequest<GetProductByKeyResponse>
    {
        public Guid Id { get; set; }
    }
} 