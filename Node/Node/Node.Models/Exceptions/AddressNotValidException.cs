using System;

namespace Node.Domain
{
     public class AddressNotValidException : Exception
    {
        public AddressNotValidException(string message) : base(message) { }
    }
}
