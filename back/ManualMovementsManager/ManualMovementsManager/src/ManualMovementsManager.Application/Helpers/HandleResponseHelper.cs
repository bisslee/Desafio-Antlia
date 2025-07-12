using Biss.MultiSinkLogger;
using FluentValidation.Results;
using ManualMovementsManager.Domain.Entities.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManualMovementsManager.Application.Helpers
{
    public class HandleResponseHelper
    {
        public HandleResponseHelper()
        {
        }

        // Generic success response
        public static void HandleResponse<T>(
            BaseResponse<T> response,
            T entity,
            int code = 200,
            string? message = null)
        {
            response.Data = entity;
            response.Success = true;
            response.StatusCode = code;
            response.Message = message;
        }

        public static void HandleNotFoundResponse<T>(
            BaseResponse<T> response,

            int code = 404,
            string? message = null)
        {
            response.Success = true;
            response.StatusCode = code;
            response.Message = message;
        }

        // General bad request response
        public static BaseResponse<T> HandleResponseBadRequest<T>(
            BaseResponse<T> response, string message)
        {
            response.Message = message;
            response.Success = false;
            response.StatusCode = 400;
            return response;
        }

        // Bad request response from ValidationFailure list
        public static BaseResponse<T> HandleResponseBadRequest<T>(
            BaseResponse<T> response, IEnumerable<ValidationFailure> errors)
        {
            var message = BuildErrorMessage(errors.Select(e => e.ErrorMessage));
            return HandleResponseBadRequest(response, message);
        }


        // Helper to handle exceptions
        public static BaseResponse<T> HandleResponseException<T>(
            BaseResponse<T> response, Exception exception, string? fallbackMessage = null)
        {
            Logger.Error(fallbackMessage ?? exception.Message, exception);
            response.Success = false;
            response.StatusCode = 500;
            response.Message = fallbackMessage ?? exception.Message;
            return response;
        }

        // Helper to build error messages
        private static string BuildErrorMessage(IEnumerable<string> errors)
        {
            var messageBuilder = new StringBuilder();
            foreach (var error in errors)
            {
                messageBuilder.AppendLine(error);
            }
            return messageBuilder.ToString();
        }
    }
}
