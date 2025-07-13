using MediatR;
using System;

namespace ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosifByKey
{
    public class GetProductCosifByKeyRequest : IRequest<GetProductCosifByKeyResponse>
    {
        public Guid Id { get; set; }
    }
} 