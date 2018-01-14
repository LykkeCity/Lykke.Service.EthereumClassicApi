﻿using System;

namespace Lykke.Service.EthereumClassicApi.Actors.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {

        }

        public NotFoundException(string message)
            : base(message)
        {

        }

        public NotFoundException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}