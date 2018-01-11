using System;

namespace Lykke.Service.EthereumClassic.Api.Common.Utils
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