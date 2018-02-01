using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Lykke.Service.EthereumClassicApi.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorResponse = ErrorResponse.Create("Bad Request");

                foreach (var state in context.ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errorResponse.AddModelError(state.Key, error.ErrorMessage);
                    }
                }

                context.Result = new BadRequestObjectResult(errorResponse);
            }

            base.OnActionExecuting(context);
        }
    }
}
