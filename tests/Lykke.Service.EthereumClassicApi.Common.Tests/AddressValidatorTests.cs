using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.EthereumClassicApi.Common.Tests
{
    [TestClass]
    public class AddressValidatorTests
    {
        [DataTestMethod]
        [DataRow("0xea674fdde714fd979de3edf0f56aa9716b898ec8", true)]  // All lowercase
        [DataRow("0xEA674FDDE714FD979DE3EDF0F56AA9716B898EC8", true)]  // All uppercase
        [DataRow("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8", true)]  // Valid checksum
        [DataRow("0xEA674fdDe714fd979de3EdF0F56aa9716B898EC8", false)] // Invalid checksum
        [DataRow("ea674fdde714fd979de3edf0f56aa9716b898ec8", false)]   // invalid format
        [DataRow("", false)]
        public async Task Validate__ExpectedResultReturned(string addressSample, bool expectedResult)
        {
            var actualResult = await AddressValidator.ValidateAsync(addressSample);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
