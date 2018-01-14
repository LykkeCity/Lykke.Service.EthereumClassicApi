using System;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;


namespace Lykke.Service.EthereumClassicApi.Mappers
{
    internal static class OperationStateDtoMappings
    {
        public static BroadcastedTransactionResponse ToBuildTransactionResponse(this OperationStateDto dto)
        {
            BroadcastedTransactionState state;

            switch (dto.State)
            {
                case OperationState.InProgress:
                    state = BroadcastedTransactionState.InProgress;
                    break;
                case OperationState.Completed:
                    state = BroadcastedTransactionState.Completed;
                    break;
                case OperationState.Failed:
                    state = BroadcastedTransactionState.Failed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new BroadcastedTransactionResponse
            {
                Amount      = dto.Amount.ToString(),
                Error       = dto.Error,
                Fee         = dto.Fee.ToString(),
                Hash        = dto.TxHash,
                OperationId = dto.OperationId,
                State       = state,
                Timestamp   = dto.Timestamp.DateTime
            };
        }
    }
}
