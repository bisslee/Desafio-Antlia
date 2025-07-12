using MediatR;
using System;

namespace ManualMovementsManager.Application.Commands.Customers.AddCustomer
{
    public class AddCustomerRequest: IRequest<AddCustomerResponse>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DocumentNumber { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public AddressRequest Address { get; set; }
        public string FavoriteSport { get; set; }
        public string FavoriteClub { get; set; }
        public bool AcceptTermsUse { get; set; }
        public bool AcceptPrivacyPolicy { get; set; }
        
    }
}
