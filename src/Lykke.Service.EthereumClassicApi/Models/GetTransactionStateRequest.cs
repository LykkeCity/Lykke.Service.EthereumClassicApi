using System;
using Lykke.Service.EthereumClassicApi.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Models
{
    public class GetTransactionStateRequest
    {
        [FromRoute, NotEmptyGuid]
        public Guid OperationId { get; set; }
    }
}
