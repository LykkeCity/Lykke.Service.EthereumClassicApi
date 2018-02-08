using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lykke.Service.EthereumClassicApi.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        private readonly HttpStatusCode _httpStatusCode;
        private readonly Type _exceptionType;

        public ExceptionFilterAttribute(Type exceptionType, HttpStatusCode httpStatusCode)
        {
            _httpStatusCode = httpStatusCode;
            _exceptionType = exceptionType;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            if (exception.GetType() == _exceptionType)
            {
                var errorResponse = new ErrorResponse();

                errorResponse.ErrorMessage = exception.Message;
                context.HttpContext.Response.StatusCode = (int)_httpStatusCode;
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse);
                byte[] data = Encoding.UTF8.GetBytes(jsonString);
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.Body.Write(data, 0, data.Length);
                context.ExceptionHandled = true;
            }
        }
    }
}
