using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain.Exceptions
{
    public class NonceUselessException : Exception
    {
        public NonceUselessException()
            : base("Nonce is useless, block already mined")
        {

        }
    }
}
