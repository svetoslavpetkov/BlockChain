using System;

namespace BlockChain.Core
{
    public class TransactionRaw
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public ulong Amount { get; set; }

        public ulong Fee { get { return 100000; }  }
        public DateTime DateCreated { get; set; }
    }
}
