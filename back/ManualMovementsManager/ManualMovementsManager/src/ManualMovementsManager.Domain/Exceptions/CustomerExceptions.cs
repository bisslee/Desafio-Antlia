using System;

namespace ManualMovementsManager.Domain.Exceptions
{
    public class CustomerNotFoundException : DomainException
    {
        public CustomerNotFoundException(Guid customerId) 
            : base($"Customer with ID '{customerId}' was not found.", "CUSTOMER_NOT_FOUND", 404)
        {
        }

        public CustomerNotFoundException(string email) 
            : base($"Customer with email '{email}' was not found.", "CUSTOMER_NOT_FOUND", 404)
        {
        }
    }

    public class CustomerEmailAlreadyExistsException : DomainException
    {
        public CustomerEmailAlreadyExistsException(string email) 
            : base($"Customer with email '{email}' already exists.", "CUSTOMER_EMAIL_ALREADY_EXISTS", 409)
        {
        }

        public CustomerEmailAlreadyExistsException(string email, Guid existingCustomerId) 
            : base($"Customer with email '{email}' already exists (ID: {existingCustomerId}).", "CUSTOMER_EMAIL_ALREADY_EXISTS", 409)
        {
        }
    }

    public class CustomerDocumentAlreadyExistsException : DomainException
    {
        public CustomerDocumentAlreadyExistsException(string documentNumber) 
            : base($"Customer with document number '{documentNumber}' already exists.", "CUSTOMER_DOCUMENT_ALREADY_EXISTS", 409)
        {
        }

        public CustomerDocumentAlreadyExistsException(string documentNumber, Guid existingCustomerId) 
            : base($"Customer with document number '{documentNumber}' already exists (ID: {existingCustomerId}).", "CUSTOMER_DOCUMENT_ALREADY_EXISTS", 409)
        {
        }
    }

    public class CustomerValidationException : DomainException
    {
        public CustomerValidationException(string message) 
            : base(message, "CUSTOMER_VALIDATION_ERROR", 400)
        {
        }

        public CustomerValidationException(string message, Exception innerException) 
            : base(message, "CUSTOMER_VALIDATION_ERROR", 400, innerException)
        {
        }
    }

    public class CustomerOperationException : DomainException
    {
        public CustomerOperationException(string operation, string message) 
            : base($"Error during customer {operation}: {message}", "CUSTOMER_OPERATION_ERROR", 500)
        {
        }

        public CustomerOperationException(string operation, string message, Exception innerException) 
            : base($"Error during customer {operation}: {message}", "CUSTOMER_OPERATION_ERROR", 500, innerException)
        {
        }
    }
} 