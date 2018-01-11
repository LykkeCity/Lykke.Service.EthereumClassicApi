using System.Threading.Tasks;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces
{
    public interface IAddStrategy<in T>
        where T : AzureTableEntity, new()
    {
        Task ExecuteAsync(T entity);
    }
}
