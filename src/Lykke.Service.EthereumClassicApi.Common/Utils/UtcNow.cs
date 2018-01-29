using System;

namespace Lykke.Service.EthereumClassicApi.Common.Utils
{
    public static class UtcNow
    {
        public static DateTime Get()
        {
            return UtcNowContext.Current == null
                ? DateTime.UtcNow
                : UtcNowContext.Current.ContextUtcNow;
        }
    }
}
