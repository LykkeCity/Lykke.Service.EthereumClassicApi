using System;

namespace Lykke.Service.EthereumClassicApi.Common.Utils
{
    public static class EnumUtil
    {
        public static T Parse<T>(string s)
        {
            return (T) Enum.Parse(typeof(T), s, true);
        }
    }
}
