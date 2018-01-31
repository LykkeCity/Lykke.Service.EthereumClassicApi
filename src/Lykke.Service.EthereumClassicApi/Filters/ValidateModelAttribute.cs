using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace Lykke.Service.EthereumClassicApi.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorResponse = new ErrorResponse();

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
