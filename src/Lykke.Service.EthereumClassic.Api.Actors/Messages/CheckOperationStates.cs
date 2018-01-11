﻿using System.ComponentModel;

namespace Lykke.Service.EthereumClassic.Api.Actors.Messages
{
    [ImmutableObject(true)]
    public class CheckOperationStates
    {
        private CheckOperationStates()
        {

        }

        /// <summary>
        ///    The singleton instance of CheckOperationStates.
        /// </summary>
        public static CheckOperationStates Instance { get; }
            = new CheckOperationStates();
    }
}
