using System;
using System.Collections.Generic;
using System.Text;

namespace BlockChain.Core
{
   public class BalanceNotEnoughException : Exception
    {
        public BalanceNotEnoughException(string message) : base(message) { }
    }
}
