using BlockChain.Core;
using System;

namespace Node.Domain.ApiModels
{
    public class GetTransactionApiModel : Transaction
    {
        public int? MinedInBlock { get; set; }

        public bool TransferSuccessFull { get { return MinedInBlock.HasValue; }  }
    }
}
