using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Util.Full
{
    public class Transaction
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        public decimal Value { get; set; }
        public DateTime DateCreated { get; set; }

        public string SenderPublicKey { get; set; }

        public string[] Signature { get; set; }

        public string TransactionHash { get; set; }
    }
}
