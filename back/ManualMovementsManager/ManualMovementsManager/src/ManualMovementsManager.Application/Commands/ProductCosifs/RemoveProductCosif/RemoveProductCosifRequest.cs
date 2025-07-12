using MediatR;
using ManualMovementsManager.Domain.Entities;
using System;

namespace ManualMovementsManager.Application.Commands.ProductCosifs.RemoveProductCosif
{
    public class RemoveProductCosifRequest : IRequest<RemoveProductCosifResponse>
    {
        public Guid Id { get; set; }
    }
} 