using System;
using System.Numerics;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class TransactionEntity : AzureTableEntity
    {
        private BigInteger _amount;
        private DateTime? _broadcastedOn;
        private DateTime _builtOn;
        private DateTime? _completedOn;
        private BigInteger _fee;
        private BigInteger _gasPrice;
        private bool _includeFee;
        private BigInteger _nonce;
        private Guid _operationId;
        private TransactionState _state;

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

        public BigInteger Fee
        {
            get
            {
                return _fee;
            }
            set
            {
                if (_fee != value)
                {
                    _fee = value;

                    MarkValueTypePropertyAsDirty(nameof(Fee));
                }
            }
        }

        public string FromAddress { get; set; }

        public BigInteger GasPrice
        {
            get
            {
                return _gasPrice;
            }
            set
            {
                if (_gasPrice != value)
                {
                    _gasPrice = value;

                    MarkValueTypePropertyAsDirty(nameof(GasPrice));
                }
            }
        }

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

        public BigInteger Nonce
        {
            get
            {
                return _nonce;
            }
            set
            {
                if (_nonce != value)
                {
                    _nonce = value;

                    MarkValueTypePropertyAsDirty(nameof(Nonce));
                }
            }
        }

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

        public TransactionState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    _state = value;

                    MarkValueTypePropertyAsDirty(nameof(State));
                }
            }
        }

        public string ToAddress { get; set; }

        public string TxData { get; set; }
    }
}
