using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Response;
using System.Collections.Generic;

namespace ManualMovementsManager.Application.Queries.Customers.GetCustomer
{
    public class GetCustomerResponse : PagedResponse<List<Customer>>
    {
    }
}
