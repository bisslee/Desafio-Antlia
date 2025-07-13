using AutoMapper;
using FluentValidation;
using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.DTOs;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Specifications.Customers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.Customers.AddCustomer
{
    public class AddCustomerHandler : IRequestHandler<AddCustomerRequest, AddCustomerResponse>
    {
        private readonly ILogger<AddCustomerHandler> Logger;
        private readonly IWriteRepository<Customer> Repository;
        private readonly ICustomerReadRepository CustomerReadRepository;
        private readonly IValidator<AddCustomerRequest> Validator;
        private readonly IMapper Mapper;
        private readonly IResponseBuilder ResponseBuilder;

        public AddCustomerHandler(
            ILogger<AddCustomerHandler> logger,
            IWriteRepository<Customer> repository,
            ICustomerReadRepository customerReadRepository,
            IValidator<AddCustomerRequest> validator,
            IMapper mapper,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            CustomerReadRepository = customerReadRepository;
            Validator = validator;
            Mapper = mapper;
            ResponseBuilder = responseBuilder;
        }

        public async Task<AddCustomerResponse> Handle(AddCustomerRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting AddCustomerRequest processing for email: {Email}", request.Email);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);

            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join("; ", errors);
                Logger.LogWarning("Validation failed for AddCustomerRequest. Email: {Email}, Errors: {Errors}", 
                    request.Email, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<AddCustomerResponse, CustomerDto>(errorMessage, errors);
            }

            try
            {
                var entity = Mapper.Map<Customer>(request);
                entity.Id = Guid.NewGuid();
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = "Template Api";

                Logger.LogDebug("Customer entity mapped successfully. ID: {CustomerId}, Email: {Email}", 
                    entity.Id, entity.Email);

                // Aplicar specifications de negócio
                var specifications = new List<IAsyncSpecification<Customer>>
                {
                    new CustomerEmailMustBeUniqueSpecification(CustomerReadRepository),
                    new CustomerDocumentMustBeUniqueSpecification(CustomerReadRepository)
                };

                Logger.LogDebug("Validating business rules for customer. Email: {Email}, Document: {Document}", 
                    entity.Email, entity.DocumentNumber);

                await SpecificationHandler.ValidateAllAsync(specifications, entity);

                Logger.LogDebug("Business rules validated successfully. Proceeding to save customer. ID: {CustomerId}", 
                    entity.Id);

                var result = await Repository.Add(entity);

                if (result)
                {
                    Logger.LogInformation("Customer created successfully. ID: {CustomerId}, Email: {Email}", 
                        entity.Id, entity.Email);
                    var dto = Mapper.Map<CustomerDto>(entity);
                    return ResponseBuilder.BuildSuccessResponse<AddCustomerResponse, CustomerDto>(dto, "Customer created successfully", 201);
                }
                
                Logger.LogError("Failed to create customer in repository. ID: {CustomerId}, Email: {Email}", 
                    entity.Id, entity.Email);
                return ResponseBuilder.BuildErrorResponse<AddCustomerResponse, CustomerDto>("Failed to create customer");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while creating customer. Email: {Email}", request.Email);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
}
