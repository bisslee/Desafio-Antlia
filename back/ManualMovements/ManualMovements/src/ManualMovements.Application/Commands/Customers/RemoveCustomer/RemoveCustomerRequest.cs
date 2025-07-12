using MediatR;
using ManualMovements.Domain.Entities;
using System;

namespace ManualMovements.Application.Commands.Customers.RemoveCustomer
{
    public class RemoveCustomerRequest : IRequest<RemoveCustomerResponse>
    {
        public Guid Id { get; set; }
       

    }
}
