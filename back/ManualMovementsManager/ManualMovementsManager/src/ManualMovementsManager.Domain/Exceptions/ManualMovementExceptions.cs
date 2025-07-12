using System;

namespace ManualMovementsManager.Domain.Exceptions
{
    public class ManualMovementNotFoundException : DomainException
    {
        public ManualMovementNotFoundException(Guid id) 
            : base($"Manual movement with ID '{id}' was not found.", "MANUAL_MOVEMENT_NOT_FOUND", 404)
        {
        }
    }

    public class ManualMovementValidationException : DomainException
    {
        public ManualMovementValidationException(string message) 
            : base(message, "MANUAL_MOVEMENT_VALIDATION_ERROR", 400)
        {
        }

        public ManualMovementValidationException(string message, Exception innerException) 
            : base(message, "MANUAL_MOVEMENT_VALIDATION_ERROR", 400, innerException)
        {
        }
    }

    public class ManualMovementOperationException : DomainException
    {
        public ManualMovementOperationException(string operation, string message) 
            : base($"Error during manual movement {operation}: {message}", "MANUAL_MOVEMENT_OPERATION_ERROR", 500)
        {
        }

        public ManualMovementOperationException(string operation, string message, Exception innerException) 
            : base($"Error during manual movement {operation}: {message}", "MANUAL_MOVEMENT_OPERATION_ERROR", 500, innerException)
        {
        }
    }
} 