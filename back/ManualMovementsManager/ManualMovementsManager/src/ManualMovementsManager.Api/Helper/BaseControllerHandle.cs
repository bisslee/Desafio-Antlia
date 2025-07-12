using Biss.MultiSinkLogger;
using ManualMovementsManager.Domain.Entities.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;
using System.Globalization;
using System.Net;

namespace ManualMovementsManager.Api.Helper
{
    public class BaseControllerHandle : ControllerBase
    {
        const string TotalCountHeader = "X-Total-Count";
        const string NullResponseMessage = "Null response received.";
        const string BadRequestMessage = "Bad Request: {@Response}";
        const string PartialContentMessage = "Partial Content: {@Response}";
        const string NoContentMessage = "No Content: {@Response}";
        const string InternalServerErrorMessage = "An error occurred: {@Message}";

        public BaseControllerHandle()
        {
        }

        [NonAction] // Evita que o Swagger considere este método como uma ação
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var acceptLanguage = context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                try
                {
                    var culture = new CultureInfo(acceptLanguage);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
                catch (CultureNotFoundException)
                {
                    Logger.Warning($"Invalid culture provided: {acceptLanguage}");
                }
            }
        }

        [NonAction] // Adicione este atributo também aqui
        public void OnActionExecuted(ActionExecutedContext context) { }

        public ActionResult HandleResponse<TEntityResponse>(BaseResponse<TEntityResponse> response)
        {
            if (response == null)
            {
                Logger.Info(NullResponseMessage);
                return BadRequest(NullResponseMessage);
            }

            if (!response.Success)
            {
                Logger.Info(BadRequestMessage, response.Message);
                return BadRequest(response);
            }

            if (response.Data == null)
            {
                Logger.Warning($"Invalid culture provided: {response}");
                return NoContent();
            }

            if (response.Data is ICollection collection)
            {
                if (collection.Count == 0)
                {
                    Logger.Info(NoContentMessage, response.Message);
                    return NoContent();
                }

                Response.Headers.Append(TotalCountHeader, collection.Count.ToString());
                Logger.Info(PartialContentMessage, response.Message);
                return new ObjectResult(response) { StatusCode = (int)HttpStatusCode.PartialContent };
            }

            return response.StatusCode switch
            {
                (int)HttpStatusCode.Created => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Created },
                (int)HttpStatusCode.NoContent => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.NoContent },
                (int)HttpStatusCode.NotFound => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.NotFound },
                _ => Ok(response)
            };
        }

        protected ActionResult HandleException(Exception ex)
        {
            Logger.Error(InternalServerErrorMessage, ex);
            return new ObjectResult(ex) { StatusCode = (int)HttpStatusCode.InternalServerError };
        }
    }

}
