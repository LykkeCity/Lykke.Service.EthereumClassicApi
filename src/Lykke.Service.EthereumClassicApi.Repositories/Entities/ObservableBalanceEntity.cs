using System.Numerics;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class ObservableBalanceEntity : AzureTableEntity
    {
        private BigInteger _amount;
        private BigInteger _blockNumber;


        public string Address { get; set; }

        public BigInteger Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                if (_amount != value)
                {
                    _amount = value;

                    MarkValueTypePropertyAsDirty(nameof(Amount));
                }
            }
        }

        public BigInteger BlockNumber
        {
            get
            {
                return _blockNumber;
            }
            set
            {
                if (_blockNumber != value)
                {
                    _blockNumber = value;

                    MarkValueTypePropertyAsDirty(nameof(BlockNumber));
                }
            }
        }
    }
}
