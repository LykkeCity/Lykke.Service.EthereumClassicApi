namespace Lykke.Service.EthereumClassicApi.Common.Utils
{
    public static class ApdexCalculator
    {
        public static double Calculate(int satisfiedCount, int toleratingCount, int totalSamplesCount)
        {
            if (totalSamplesCount == 0)
            {
                return 0;
            }

            return (satisfiedCount + (double) toleratingCount / 2) / totalSamplesCount;
        }
    }
}
