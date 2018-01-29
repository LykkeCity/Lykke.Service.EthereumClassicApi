using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Services
{
    public class AddressValidationService : IAddressValidationService
    {
        private readonly IEthereum _ethereum;


        public AddressValidationService(
            IEthereum ethereum)
        {
            _ethereum = ethereum;
        }


        public async Task<bool> ValidateAddressAsync(string address)
        {
            if (await AddressValidator.ValidateAsync(address))
            {
                var addressCode = await _ethereum.GetCodeAsync(address);

                return addressCode == "0x";
            }

            return false;
        }
    }
}
