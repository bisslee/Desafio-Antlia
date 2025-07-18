﻿using System;

namespace ManualMovementsManager.Domain.Entities
{
    public class Address : BaseEntity
    {
        
        public Guid CustomerId { get; set; } // Foreign key
        public Customer Customer { get; set; } // Navigation property
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

    }
}