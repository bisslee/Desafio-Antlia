using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManualMovementsManager.Application.Queries.Customers.GetCustomerByKey
{
    public class GetCustomerByKeyRequest : IRequest<GetCustomerByKeyResponse>
    {
        public Guid Id { get; set; }
    }
}
