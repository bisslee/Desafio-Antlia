using ManualMovements.Domain.Entities;
using ManualMovements.Domain.Entities.Response;
using System.Collections.Generic;

namespace ManualMovements.Application.Queries.Customers.GetCustomer
{
    public class GetCustomerResponse : PagedResponse<List<Customer>>
    {
    }
}
