using Lykke.Service.EthereumClassicApi.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Models
{
    public class DeleteAddressFromObservationListRequest
    {
        [FromRoute, Address]
        public string Address { get; set; }
    }
}
