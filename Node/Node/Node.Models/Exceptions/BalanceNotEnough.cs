using System;

namespace Node.Domain.Exceptions
{
    public class BalanceNotEnough : Exception
    {
        public BalanceNotEnough(string message) : base(message) { }
    }
}
