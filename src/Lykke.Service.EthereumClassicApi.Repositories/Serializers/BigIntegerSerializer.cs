using System;
using System.Numerics;
using Lykke.AzureStorage.Tables.Entity.Serializers;

namespace Lykke.Service.EthereumClassicApi.Repositories.Serializers
{
    public class BigIntegerSerializer : IStorageValueSerializer
    {
        public string Serialize(object value, Type type)
        {
            return value.ToString();
        }

        public object Deserialize(string serialized, Type type)
        {
            return BigInteger.Parse(serialized);
        }
    }
}
