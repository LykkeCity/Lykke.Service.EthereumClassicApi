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
        private bool _locked;


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

        public bool Locked
        {
            get
            {
                return _locked;
            }
            set
            {
                if (_locked != value)
                {
                    _locked = value;

                    MarkValueTypePropertyAsDirty(nameof(Locked));
                }
            }
        }
    }
}
