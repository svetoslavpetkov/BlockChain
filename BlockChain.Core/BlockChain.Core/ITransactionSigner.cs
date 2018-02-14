using System;

namespace BlockChain.Core
{
    public interface ITransactionSigner
    {
        Transaction Sign(string privateKey, string recipientAddress, decimal amount, DateTime signDate);
        string CalculateAddress(string privateKey);
    }

}
