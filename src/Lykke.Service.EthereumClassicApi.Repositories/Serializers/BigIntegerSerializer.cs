using System.Numerics;
using Lykke.AzureStorage.Tables.Entity.Serializers;

namespace Lykke.Service.EthereumClassicApi.Repositories.Serializers
{
    public class BigIntegerSerializer : IStorageValueSerializer
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public object Deserialize(string serialized)
        {
            return BigInteger.Parse(serialized);
        }
    }
}
