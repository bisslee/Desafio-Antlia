﻿using System;

namespace ManualMovementsManager.Application.Commands
{
    public class AddressRequest
    {
        public string Street { get; set; } = string.Empty;
        public string? Number { get; set; } = string.Empty;
        public string? Complement { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }
}
