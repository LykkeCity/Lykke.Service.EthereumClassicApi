namespace Lykke.Service.EthereumClassic.Api.Common.Utils
{
    public static class ApdexCalculator
    {
        public static double Calculate(int satisfiedCount, int toleratingCount, int totalSamplesCount)
        {
            if (totalSamplesCount == 0)
            {
                return 0;
            }
            else
            {
                return (satisfiedCount + ((double) toleratingCount / 2)) / totalSamplesCount;
            }
        }
    }
}
