using System;

namespace BlockChain.Core
{
    public interface ITransactionSigner
    {
        Transaction Sign(string privateKey, string recipientAddress, ulong amount, DateTime signDate);
        string CalculateAddress(string privateKey);
    }

}
