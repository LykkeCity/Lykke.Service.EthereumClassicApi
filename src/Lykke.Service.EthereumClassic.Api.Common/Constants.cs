﻿using System.Numerics;

namespace Lykke.Service.EthereumClassic.Api.Common
{
    public static class Constants
    {
        public const string ApplicationName
            = "EthereumClassicApi";
        
        public static BigInteger EtcTransferGasAmount 
            = new BigInteger(21000);

        public const bool IsDebug
#if DEBUG
            = true;
#else
            = false;
#endif


        public static class EtcAsset
        {
            public const int Accuracy
                = 18;

            public const string AssetId
                = "ETC";

            public const string Name
                = "Ethereum Classic";
        }
    }
}
