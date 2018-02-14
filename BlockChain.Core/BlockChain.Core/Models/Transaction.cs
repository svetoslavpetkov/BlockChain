
namespace BlockChain.Core
{
    public class Transaction : TransactionRaw
    {
        public string TransactionHash { get; set; }
        public string SenderPublicKey { get; set; }
        public string[] Signature { get; set; }
        public bool TranserSuccessfull { get; set; }
    }
}
