using ManualMovements.Domain.Repositories;
using ManualMovements.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace ManualMovements.Domain.Specifications.Customers
{
    /// <summary>
    /// Specification que valida se o email do customer é único
    /// </summary>
    public class CustomerEmailMustBeUniqueSpecification : IAsyncSpecification<ManualMovements.Domain.Entities.Customer>
    {
        private readonly ICustomerReadRepository CustomerRepository;
        private readonly Guid? ExcludeCustomerId;

        public CustomerEmailMustBeUniqueSpecification(ICustomerReadRepository customerRepository, Guid? excludeCustomerId = null)
        {
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            ExcludeCustomerId = excludeCustomerId;
        }

        public bool IsSatisfiedBy(ManualMovements.Domain.Entities.Customer customer)
        {
            // Para compatibilidade, mas não deve ser usado
            throw new NotImplementedException("Use IsSatisfiedByAsync for async operations");
        }

        public async Task<bool> IsSatisfiedByAsync(ManualMovements.Domain.Entities.Customer customer)
        {
            if (customer == null || string.IsNullOrWhiteSpace(customer.Email))
                return true;

            var existingCustomer = await CustomerRepository.GetByEmailAsync(customer.Email);

            if (existingCustomer == null)
                return true;

            if (ExcludeCustomerId.HasValue && 
                existingCustomer.Id == ExcludeCustomerId.Value)
                return true;

            throw new CustomerEmailAlreadyExistsException(customer.Email, existingCustomer.Id);
        }

        public string ErrorMessage => "Email already exists.";
    }
}