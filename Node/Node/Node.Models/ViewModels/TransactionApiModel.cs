using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Models.ViewModels
{
    public class TransactionApiModel
    {
        public string From { get; private set; }
        public string To { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string SenderPublicKey { get; private set; }
        public string[] Signature { get; private set; }
        public string Hash { get; private set; }
    }
}
