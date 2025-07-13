using MediatR;
using System;

namespace ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovement
{
    public class GetManualMovementRequest : BaseRequest, IRequest<GetManualMovementResponse>
    {
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public bool? Active { get; set; }
    }
} 