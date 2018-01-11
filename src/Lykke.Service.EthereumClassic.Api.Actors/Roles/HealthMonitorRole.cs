using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassic.Api.Common.Settings;
using Lykke.Service.EthereumClassic.Api.Common.Utils;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors.Roles
{
    public class HealthMonitorRole : IHealthMonitorRole
    {
        private readonly List<(DateTime MeasurementTime, bool Failed)> _cashins;
        private readonly List<(DateTime MeasurementTime, bool Failed)> _cashouts;
        private readonly IHealthStatusRepository                       _healthStatusRepository;
        private readonly EthereumClassicApiSettings                    _settings;


        public HealthMonitorRole(
            IHealthStatusRepository healthStatusRepository,
            EthereumClassicApiSettings settings)
        {
            _cashins                = new List<(DateTime MeasurementTime, bool Failed)>();
            _cashouts               = new List<(DateTime MeasurementTime, bool Failed)>();
            _healthStatusRepository = healthStatusRepository;
            _settings               = settings;
        }


        public async Task UpdateHealthStatusAsync()
        {
            //CleanupApdexMeasurements();

            var healthStatus = _healthStatusRepository.Get();

            //healthStatus.CashinApdex                  = CalculateCashinApdex();
            //healthStatus.CashoutApdex                 = CalculateCashoutApdex();
            //healthStatus.EthereumRpcNodeStatusIsAlive = await CheckEthereumRpcNodeStatusAsync();
            //healthStatus.EthereumRpcNodeStatusIsAlive = await CheckSignServiceStatusAsync();

            _healthStatusRepository.Update(healthStatus);
        }

        public void RegisterCashin(bool failed)
        {
            _cashins.Add((UtcNow.Get(), failed));
        }

        public void RegisterCashout(bool failed)
        {
            _cashouts.Add((UtcNow.Get(), failed));
        }
        
        //private double CalculateCashinApdex()
        //{
        //    var satisfiedCount  = _cashins.Count(x => !x.Failed);
        //    var toleratingCount = _settings.CashinApdexToleratingCount;
        //    var totalSamples    = _cashins.Count;
        //
        //    return ApdexCalculator.Calculate(satisfiedCount, toleratingCount, totalSamples);
        //}

        //private double CalculateCashoutApdex()
        //{
        //    var satisfiedCount  = _cashouts.Count(x => !x.Failed);
        //    var toleratingCount = _settings.CashoutApdexToleratingCount;
        //    var totalSamples    = _cashouts.Count;
        //
        //    return ApdexCalculator.Calculate(satisfiedCount, toleratingCount, totalSamples);
        //}

        //private async Task<bool> CheckEthereumRpcNodeStatusAsync()
        //{
        //    //TODO: Add implementation
        //
        //    return true;
        //}

        //private async Task<bool> CheckSignServiceStatusAsync()
        //{
        //    //TODO: Add implementation
        //
        //    return true;
        //}

        //private void CleanupApdexMeasurements()
        //{
        //    _cashins
        //        .RemoveAll(x => x.MeasurementTime < UtcNow.Get().Subtract(_settings.CashinApdexPeriod));
        //
        //    _cashouts
        //        .RemoveAll(x => x.MeasurementTime < UtcNow.Get().Subtract(_settings.CashoutApdexPeriod));
        //}
    }
}
