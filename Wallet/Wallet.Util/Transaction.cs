using System;

namespace Wallet.Util
{
    public class TransactionRaw
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }

    }


    public class SignedTransaction: TransactionRaw
    {
        public string SenderPublicKey { get; set; }

        public string[] Signature { get; set; }
    }


   
    public class Transaction: SignedTransaction
    {
        public string TransactionHash { get; set; }
    }

}