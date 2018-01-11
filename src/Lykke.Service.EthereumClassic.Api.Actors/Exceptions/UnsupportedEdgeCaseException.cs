using System;

namespace Lykke.Service.EthereumClassic.Api.Actors.Exceptions
{
    public class UnsupportedEdgeCaseException : Exception
    {
        public UnsupportedEdgeCaseException()
        {

        }

        public UnsupportedEdgeCaseException(string message)
            : base(message)
        {

        }

        public UnsupportedEdgeCaseException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
