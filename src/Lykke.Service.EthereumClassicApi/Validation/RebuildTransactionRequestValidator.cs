using FluentValidation;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using System;

namespace Lykke.Service.EthereumClassicApi.Validation
{
    public class RebuildTransactionRequestValidator : AbstractValidator<RebuildTransactionRequest>
    {
        public RebuildTransactionRequestValidator()
        {
            RuleFor(x => x.OperationId)
                .Must((operationId) => operationId != Guid.Empty)
                .WithMessage(x => $"OperationId should not be empty.");

            RuleFor(x => x.FeeFactor)
                .Must((feeFactor) => feeFactor > 1m)
                .WithMessage(x => $"FeeFactor [{x.FeeFactor}] should be greater then 1.");
        }
    }
}
