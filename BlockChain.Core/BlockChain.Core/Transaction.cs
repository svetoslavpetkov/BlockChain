﻿using System;

namespace BlockChain.Core
{
    public class TransactionRaw
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class Transaction : TransactionRaw
    {
        public string TransactionHash { get; set; }

        public string SenderPublicKey { get; set; }

        public string[] Signature { get; set; }
    }
}
