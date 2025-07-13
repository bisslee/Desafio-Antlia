using MediatR;
using System;

namespace ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovementByKey
{
    public class GetManualMovementByKeyRequest : IRequest<GetManualMovementByKeyResponse>
    {
        public Guid Id { get; set; }
    }
} 