using BlockChain.Core;
using System;

namespace Node.Domain.ApiModels
{
    public enum TransactionStatus
    {
        Pending = 0,
        Declined = 1,
        Approved = 2
    }

    public class GetTransactionApiModel : Transaction
    {
        public int? MinedInBlock { get; set; }

        public string AmountString { get; set; }

        public string FeeString { get; set; }

        public TransactionStatus Status
        {
            get
            {
                if (!this.MinedInBlock.HasValue)
                {
                    return TransactionStatus.Pending;
                }
                else
                {
                    return (this.TranserSuccessfull ? TransactionStatus.Approved : TransactionStatus.Declined);
                }
            }
        }


        public static GetTransactionApiModel FromTransaction(Transaction input, int? minedInBlock = null)
        {
            return new GetTransactionApiModel()
            {
                AmountString = input.Amount.GetFormattedTokens(),
                Amount = input.Amount,
                DateCreated = input.DateCreated,
                FromAddress = input.FromAddress,
                SenderPublicKey = input.SenderPublicKey,
                Signature = input.Signature,
                ToAddress = input.ToAddress,
                TransactionHash = input.TransactionHash,
                MinedInBlock = minedInBlock,
                FeeString = input.Fee.GetFormattedTokens(),
                TranserSuccessfull = input.TranserSuccessfull
            };
        }
    }
}
