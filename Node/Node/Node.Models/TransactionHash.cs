using System;

namespace Node.Models
{
    internal class TransactionHash
    {
        public string From { get; private set; }
        public string To { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string SenderPublicKey { get; private set; }
        public string[] Signature { get; private set; }

        internal TransactionHash(string from, string to, decimal amount, DateTime creation,
            string senderPyblicKey, string[] signature)
        {
            From = from;
            To = to;
            Amount = amount;
            DateCreated = creation;
            SenderPublicKey = senderPyblicKey;
            Signature = signature;
        }
    }
}
