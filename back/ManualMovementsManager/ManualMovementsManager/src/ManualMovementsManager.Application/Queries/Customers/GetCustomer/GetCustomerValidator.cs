using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Queries.Customers.GetCustomer
{
    public class GetCustomerValidator : AbstractValidator<GetCustomerRequest>
    {
        public GetCustomerValidator()
        {
             
        }
    }
}
