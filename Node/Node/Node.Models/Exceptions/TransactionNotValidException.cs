using System;

namespace Node.Models.Exceptions
{
    public class TransactionNotValidException : Exception
    {
        public TransactionNotValidException(string message) : base(message) { }
    }
}
