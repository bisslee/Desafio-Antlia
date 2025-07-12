using ManualMovements.Domain.Repositories;
using ManualMovements.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace ManualMovements.Domain.Specifications.Customers
{
    /// <summary>
    /// Specification que valida se o customer existe
    /// </summary>
    public class CustomerMustExistSpecification : IAsyncSpecification<ManualMovements.Domain.Entities.Customer>
    {
        private readonly ICustomerReadRepository CustomerRepository;

        public CustomerMustExistSpecification(ICustomerReadRepository customerRepository)
        {
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        public bool IsSatisfiedBy(ManualMovements.Domain.Entities.Customer customer)
        {
            // Para compatibilidade, mas não deve ser usado
            throw new NotImplementedException("Use IsSatisfiedByAsync for async operations");
        }

        public async Task<bool> IsSatisfiedByAsync(ManualMovements.Domain.Entities.Customer customer)
        {
            if (customer == null || customer.Id == Guid.Empty)
                throw new CustomerNotFoundException("Invalid customer ID provided.");

            var existingCustomer = await CustomerRepository.GetByIdAsync(customer.Id);
            
            if (existingCustomer == null)
                throw new CustomerNotFoundException(customer.Id);

            return true;
        }

        public string ErrorMessage => "Client not found.";
    }
} 