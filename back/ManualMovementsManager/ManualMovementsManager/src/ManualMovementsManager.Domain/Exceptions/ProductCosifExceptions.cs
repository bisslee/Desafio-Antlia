using System;

namespace ManualMovementsManager.Domain.Exceptions
{
    public class ProductCosifNotFoundException : DomainException
    {
        public ProductCosifNotFoundException(string productCode, string cosifCode) 
            : base($"Product COSIF with product code '{productCode}' and COSIF code '{cosifCode}' was not found.", "PRODUCT_COSIF_NOT_FOUND", 404)
        {
        }
    }

    public class ProductCosifCodeAlreadyExistsException : DomainException
    {
        public ProductCosifCodeAlreadyExistsException(string productCode, string cosifCode) 
            : base($"Product COSIF with product code '{productCode}' and COSIF code '{cosifCode}' already exists.", "PRODUCT_COSIF_CODE_ALREADY_EXISTS", 409)
        {
        }
    }

    public class ProductCosifValidationException : DomainException
    {
        public ProductCosifValidationException(string message) 
            : base(message, "PRODUCT_COSIF_VALIDATION_ERROR", 400)
        {
        }

        public ProductCosifValidationException(string message, Exception innerException) 
            : base(message, "PRODUCT_COSIF_VALIDATION_ERROR", 400, innerException)
        {
        }
    }

    public class ProductCosifOperationException : DomainException
    {
        public ProductCosifOperationException(string operation, string message) 
            : base($"Error during product COSIF {operation}: {message}", "PRODUCT_COSIF_OPERATION_ERROR", 500)
        {
        }

        public ProductCosifOperationException(string operation, string message, Exception innerException) 
            : base($"Error during product COSIF {operation}: {message}", "PRODUCT_COSIF_OPERATION_ERROR", 500, innerException)
        {
        }
    }
} 