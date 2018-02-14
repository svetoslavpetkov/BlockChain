
namespace BlockChain.Core
{
    public interface ITransactionValidator
    {
        bool IsValid(Transaction transaction);
        bool IsValidateHash(Transaction transaction);
        string GetAddress(string publicKey);
    }

}
