using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Models
{
    public class Transaction
    {
        public string SenderAddress { get; private set; }
        public string ReceiverAddress { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string SenderPublicKey { get; private set; }
        public string[] Signature { get; private set; }
        public string TransactionHash { get; private set; }

        public int MinedInIndex { get; private set; }
        public bool TranserSuccessful { get; private set; }
    }
}
