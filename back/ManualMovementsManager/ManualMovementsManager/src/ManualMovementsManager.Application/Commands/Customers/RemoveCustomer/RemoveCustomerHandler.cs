using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Specifications.Customers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.Customers.RemoveCustomer
{
    public class RemoveCustomerHandler :
        IRequestHandler<RemoveCustomerRequest, RemoveCustomerResponse>
    {
        private readonly ILogger<RemoveCustomerHandler> Logger;
        private readonly IWriteRepository<Customer> Repository;
        private readonly ICustomerReadRepository CustomerReadRepository;
        private readonly IResponseBuilder ResponseBuilder;
        
        public RemoveCustomerHandler(
            ILogger<RemoveCustomerHandler> logger,
            IWriteRepository<Customer> repository,
            ICustomerReadRepository customerReadRepository,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            CustomerReadRepository = customerReadRepository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<RemoveCustomerResponse> Handle(RemoveCustomerRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting RemoveCustomerRequest processing for customer ID: {CustomerId}", request.Id);

            try
            {
                Logger.LogDebug("Checking if customer exists for removal. CustomerId: {CustomerId}", request.Id);
                
                // Verificar se o customer existe
                var customerExistsSpec = new CustomerMustExistSpecification(CustomerReadRepository);
                await SpecificationHandler.ValidateAsync(customerExistsSpec, new Customer { Id = request.Id });

                Logger.LogDebug("Customer found, retrieving data for removal. CustomerId: {CustomerId}", request.Id);
                var entity = await CustomerReadRepository.GetCustomerWithAddressByIdAsync(request.Id);
                
                Logger.LogDebug("Proceeding to delete customer. CustomerId: {CustomerId}, Email: {Email}", 
                    entity.Id, entity.Email);
                
                var result = await Repository.Delete(entity);
                
                if (result)
                {
                    Logger.LogInformation("Customer removed successfully. CustomerId: {CustomerId}, Email: {Email}", 
                        entity.Id, entity.Email);
                    return ResponseBuilder.BuildSuccessResponse<RemoveCustomerResponse, bool>(true, "Customer removed successfully");
                }

                Logger.LogError("Failed to remove customer from repository. CustomerId: {CustomerId}, Email: {Email}", 
                    entity.Id, entity.Email);
                return ResponseBuilder.BuildErrorResponse<RemoveCustomerResponse, bool>("Failed to remove customer");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while removing customer. CustomerId: {CustomerId}", request.Id);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
}
