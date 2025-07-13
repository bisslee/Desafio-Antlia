using MediatR;
using ManualMovementsManager.Domain.Entities;
using System;

namespace ManualMovementsManager.Application.Commands.Products.RemoveProduct
{
    public class RemoveProductRequest : IRequest<RemoveProductResponse>
    {
        public Guid Id { get; set; }
    }
} 