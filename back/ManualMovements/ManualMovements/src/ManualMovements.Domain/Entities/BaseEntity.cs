using ManualMovements.Domain.Entities.Enums;
using System;

namespace ManualMovements.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "System";
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "System";

        public DataStatus Status { get; set; } = DataStatus.Created;

    }
}
