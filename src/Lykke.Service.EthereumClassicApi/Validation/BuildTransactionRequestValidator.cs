using FluentValidation;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumClassicApi.Validation
{
    public class BuildTransactionRequestValidator : AbstractValidator<BuildTransactionRequest>
    {
        public BuildTransactionRequestValidator()
        {
            RuleFor(x => x.Amount)
                .Must((amount) => BigInteger.TryParse(amount, out var amountParsed) && amountParsed > 0)
                .WithMessage(x => $"Amount [{x.Amount}] should be a positive integer.");

            RuleFor(x => x.FromAddress)
                .Must((fromAddress) => AddressValidator.ValidateAsync(fromAddress).Result)
                .WithMessage(x => $"FromAddress [{x.FromAddress}] should be a valid address.");

            RuleFor(x => x.ToAddress)
                .Must((toAddress) => AddressValidator.ValidateAsync(toAddress).Result)
                .WithMessage(x => $"ToAddress [{x.ToAddress}] should be a valid address.");

            RuleFor(x => x.AssetId)
                .Must((assetId) => assetId == Constants.EtcAsset.AssetId)
                .WithMessage(x => $"AssetId [{x.AssetId}] is not supported.");
        }
    }
}
