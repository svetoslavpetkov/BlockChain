using BlockChain.Core;
using System;

namespace Node.Domain.ApiModels
{
    public class GetTransactionApiModel : Transaction
    {
        public int? MinedInBlock { get; set; }

        public bool TransferSuccessFull { get { return MinedInBlock.HasValue; }  }


        public static GetTransactionApiModel FromTransaction(Transaction input, int? minedInBlock = null)
        {
            return new GetTransactionApiModel() {
                Amount = input.Amount,
                DateCreated = input.DateCreated,
                FromAddress = input.FromAddress,
                SenderPublicKey = input.SenderPublicKey,
                Signature = input.Signature,
                ToAddress = input.ToAddress,
                TransactionHash = input.TransactionHash,
                MinedInBlock = minedInBlock
            };
        }
    }
}
