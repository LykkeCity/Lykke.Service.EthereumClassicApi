using System;
using FluentValidation;
using Lykke.Service.BlockchainApi.Contract.Transactions;

namespace Lykke.Service.EthereumClassicApi.Validation
{
    public class BroadcastTransactionRequestValidator : AbstractValidator<BroadcastTransactionRequest>
    {
        public BroadcastTransactionRequestValidator()
        {
            RuleFor(x => x.OperationId)
                .Must((operationId) => operationId != Guid.Empty)
                .WithMessage(x => $"OperationId should not be empty.");
        }
    }
}
