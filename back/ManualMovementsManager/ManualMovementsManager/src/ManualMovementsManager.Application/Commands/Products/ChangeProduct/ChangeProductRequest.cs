using MediatR;
using System;

namespace ManualMovementsManager.Application.Commands.Products.ChangeProduct
{
    public class ChangeProductRequest : IRequest<ChangeProductResponse>
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }
} 