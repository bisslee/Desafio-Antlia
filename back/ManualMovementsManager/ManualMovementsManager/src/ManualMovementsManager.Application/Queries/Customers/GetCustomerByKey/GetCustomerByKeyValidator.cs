using FluentValidation;
using ManualMovementsManager.Application.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Queries.Customers.GetCustomerByKey
{
    public class GetCustomerByKeyValidator: AbstractValidator<GetCustomerByKeyRequest>
    {
        public GetCustomerByKeyValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
}
