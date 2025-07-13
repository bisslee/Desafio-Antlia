using System;
using ManualMovementsManager.Domain.Entities.Enums;

namespace ManualMovementsManager.Application.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DocumentNumber { get; set; } = null!;
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string FavoriteSport { get; set; } = string.Empty;
        public string FavoriteClub { get; set; } = string.Empty;
        public bool AcceptTermsUse { get; set; }
        public bool AcceptPrivacyPolicy { get; set; }
    }
} 