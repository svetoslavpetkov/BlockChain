using System;
using System.Collections.Generic;

namespace Wallet.Util
{
    public class Transaction
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        public int Value { get; set; }
        public DateTime DateCreated { get; set; }

        public string SenderPublicKey { get; set; }

        public string[] Signature { get; set; }

        public string TransactionHash { get; set; }
    }
}