using System;

namespace Node.Domain.Exceptions
{
    public class TransactionNotValidException : Exception
    {
        public TransactionNotValidException(string message) : base(message) { }
    }
}
