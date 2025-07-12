using FluentValidation;
using ManualMovements.Application.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualMovements.Application.Queries.Customers.GetCustomerByKey
{
    public class GetCustomerByKeyValidator: AbstractValidator<GetCustomerByKeyRequest>
    {
        public GetCustomerByKeyValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
}
