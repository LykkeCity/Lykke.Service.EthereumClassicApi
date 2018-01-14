using System;

namespace Lykke.Service.EthereumClassicApi.Common.Utils
{
    public static class NewGuid
    {
        public static Guid Get()
        {
            return NewGuidContext.Current == null 
                 ? Guid.NewGuid()
                 : NewGuidContext.Current.ContextNewGuid;
        }
    }
}