using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class ObservableBalanceEntity : AzureTableEntity
    {
        private bool _locked;


        public string Address { get; set; }

        public string Amount { get; set; }

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
