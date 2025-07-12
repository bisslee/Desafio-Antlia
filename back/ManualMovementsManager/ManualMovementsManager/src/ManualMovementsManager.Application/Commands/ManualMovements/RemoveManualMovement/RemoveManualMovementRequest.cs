using MediatR;
using ManualMovementsManager.Domain.Entities;
using System;

namespace ManualMovementsManager.Application.Commands.ManualMovements.RemoveManualMovement
{
    public class RemoveManualMovementRequest : IRequest<RemoveManualMovementResponse>
    {
        public Guid Id { get; set; }
    }
} 