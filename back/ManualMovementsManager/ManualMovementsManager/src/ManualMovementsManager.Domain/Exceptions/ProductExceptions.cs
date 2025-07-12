using System;

namespace ManualMovementsManager.Domain.Exceptions
{
    public class ProductNotFoundException : DomainException
    {
        public ProductNotFoundException(string productCode) 
            : base($"Product with code '{productCode}' was not found.", "PRODUCT_NOT_FOUND", 404)
        {
        }

        public ProductNotFoundException(Guid id) 
            : base($"Product with ID '{id}' was not found.", "PRODUCT_NOT_FOUND", 404)
        {
        }
    }

    public class ProductCodeAlreadyExistsException : DomainException
    {
        public ProductCodeAlreadyExistsException(string productCode) 
            : base($"Product with code '{productCode}' already exists.", "PRODUCT_CODE_ALREADY_EXISTS", 409)
        {
        }
    }

    public class ProductValidationException : DomainException
    {
        public ProductValidationException(string message) 
            : base(message, "PRODUCT_VALIDATION_ERROR", 400)
        {
        }

        public ProductValidationException(string message, Exception innerException) 
            : base(message, "PRODUCT_VALIDATION_ERROR", 400, innerException)
        {
        }
    }

    public class ProductOperationException : DomainException
    {
        public ProductOperationException(string operation, string message) 
            : base($"Error during product {operation}: {message}", "PRODUCT_OPERATION_ERROR", 500)
        {
        }

        public ProductOperationException(string operation, string message, Exception innerException) 
            : base($"Error during product {operation}: {message}", "PRODUCT_OPERATION_ERROR", 500, innerException)
        {
        }
    }
} 