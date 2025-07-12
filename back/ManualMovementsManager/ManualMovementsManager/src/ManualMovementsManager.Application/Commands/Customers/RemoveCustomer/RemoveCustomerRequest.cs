using MediatR;
using ManualMovementsManager.Domain.Entities;
using System;

namespace ManualMovementsManager.Application.Commands.Customers.RemoveCustomer
{
    public class RemoveCustomerRequest : IRequest<RemoveCustomerResponse>
    {
        public Guid Id { get; set; }
       

    }
}
