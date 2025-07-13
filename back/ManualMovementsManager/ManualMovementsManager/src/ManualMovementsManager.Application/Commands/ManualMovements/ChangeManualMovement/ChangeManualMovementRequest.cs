using MediatR;
using System;

namespace ManualMovementsManager.Application.Commands.ManualMovements.ChangeManualMovement
{
    public class ChangeManualMovementRequest : IRequest<ChangeManualMovementResponse>
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string ProductCode { get; set; } = null!;
        public string CosifCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime MovementDate { get; set; }
        public string UserCode { get; set; } = null!;
        public decimal Value { get; set; }
    }
} 