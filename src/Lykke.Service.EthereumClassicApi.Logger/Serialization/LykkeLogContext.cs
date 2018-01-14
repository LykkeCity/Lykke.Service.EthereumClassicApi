namespace Lykke.Service.EthereumClassicApi.Logger.Serialization
{
    internal class LykkeLogContext
    {
        public long? Duration { get; set; }

        public string Thread { get; set; }

        public object Trigger { get; set; }
    }
}