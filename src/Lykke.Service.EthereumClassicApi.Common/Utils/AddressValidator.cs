using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Multiformats.Hash;
using Multiformats.Hash.Algorithms;

namespace Lykke.Service.EthereumClassicApi.Common.Utils
{
    public static class AddressValidator
    {
        [Pure]
        public static async Task <bool> ValidateAsync(string address)
        {
            // Check basic requirements of an address
            if (!Regex.IsMatch(address, @"^0x[0-9a-f]{40}$", RegexOptions.IgnoreCase))
            {
                return false;
            }
            
            // Check if all letters are either in lower case, or in upper case
            if (Regex.IsMatch(address, @"^0x(?:[0-9a-f]{40}|[0-9A-F]{40})$"))
            {
                return true;
            }
            
            return await ValidateChecksumAsync(address);
        }

        private static async Task<bool> ValidateChecksumAsync(string address)
        {
            address = address.Remove(0, 2);

            var addressBytes = Encoding.UTF8.GetBytes(address.ToLowerInvariant());
            var caseMapBytes = (await Multihash.SumAsync<KECCAK_256>(addressBytes)).Digest;
            
            for (var i = 0; i < 40; i++)
            {
                var addressChar = address[i];

                if (!char.IsLetter(addressChar))
                {
                    continue;
                }
                
                var leftShift     = i % 2 == 0 ? 7 : 3;
                var shouldBeUpper = (caseMapBytes[i / 2] & (1 << leftShift)) != 0;
                var shouldBeLower = !shouldBeUpper;

                if (shouldBeUpper && char.IsLower(addressChar) ||
                    shouldBeLower && char.IsUpper(addressChar))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
