using System;

namespace Lykke.Service.EthereumClassicApi.Common.Exceptions
{
    public class UnexpectedResponseException : Exception
    {
        public UnexpectedResponseException(object response)
        {
            Response = response;
        }

        public UnexpectedResponseException(object response, string message)
            : base(message)
        {
            Response = response;
        }

        public UnexpectedResponseException(object response, string message, Exception inner)
            : base(message, inner)
        {
            Response = response;
        }

        public object Response { get; }
    }
}
