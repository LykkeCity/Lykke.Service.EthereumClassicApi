using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class TransactionEntity : AzureTableEntity
    {
        private DateTime? _broadcastedOn;
        private DateTime _builtOn;
        private DateTime? _completedOn;
        private bool _includeFee;
        private Guid _operationId;

        public string Amount { get; set; }

        public DateTime? BroadcastedOn
        {
            get
            {
                return _broadcastedOn;
            }
            set
            {
                if (_broadcastedOn != value)
                {
                    _broadcastedOn = value;

                    MarkValueTypePropertyAsDirty(nameof(BroadcastedOn));
                }
            }
        }

        public DateTime BuiltOn
        {
            get
            {
                return _builtOn;
            }
            set
            {
                if (_builtOn != value)
                {
                    _builtOn = value;

                    MarkValueTypePropertyAsDirty(nameof(BuiltOn));
                }
            }
        }

        public DateTime? CompletedOn
        {
            get
            {
                return _completedOn;
            }
            set
            {
                if (_completedOn != value)
                {
                    _completedOn = value;

                    MarkValueTypePropertyAsDirty(nameof(CompletedOn));
                }
            }
        }

        public string Error { get; set; }

        public string Fee { get; set; }

        public string FromAddress { get; set; }

        public string GasPrice { get; set; }

        public bool IncludeFee
        {
            get
            {
                return _includeFee;
            }
            set
            {
                if (_includeFee != value)
                {
                    _includeFee = value;

                    MarkValueTypePropertyAsDirty(nameof(IncludeFee));
                }
            }
        }

        public string Nonce { get; set; }

        public Guid OperationId
        {
            get
            {
                return _operationId;
            }
            set
            {
                if (_operationId != value)
                {
                    _operationId = value;

                    MarkValueTypePropertyAsDirty(nameof(OperationId));
                }
            }
        }

        public string SignedTxData { get; set; }

        public string SignedTxHash { get; set; }

        public string State { get; set; }

        public string ToAddress { get; set; }

        public string TxData { get; set; }
    }
}
