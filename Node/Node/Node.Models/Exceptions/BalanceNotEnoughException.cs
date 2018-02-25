using System;

namespace Node.Domain
{
   public class BalanceNotEnoughException : Exception
    {
        public BalanceNotEnoughException(string message) : base(message) { }
    }
}
