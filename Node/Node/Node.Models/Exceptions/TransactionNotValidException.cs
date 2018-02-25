using System;

namespace Node.Domain
{
    public class TransactionNotValidException : Exception
    {
        public TransactionNotValidException(string message) : base(message) { }
    }
}
