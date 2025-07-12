using ManualMovements.Domain.Entities.Enums;
using System;

namespace ManualMovements.Domain.Entities
{

    public class Customer : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DocumentNumber { get; set; } = null!;
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; } = new Address();
        public string FavoriteSport { get; set; } = string.Empty;
        public string FavoriteClub { get; set; } = string.Empty;
        public bool AcceptTermsUse { get; set; }
        public bool AcceptPrivacyPolicy { get; set; }
    }
}
