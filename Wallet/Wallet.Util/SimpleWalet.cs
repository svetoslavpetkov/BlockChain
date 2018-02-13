using BlockChain.Core;
using System;

namespace Wallet.Util.Core
{
    public class SimpleWalet
    {
        private ITransactionSigner Signer = new TransactionSigner();
        private ICryptoUtil CryptoUtil = new CryptoUtil();

        public string PrivateKey { get; set; }

        public SimpleWalet(string privateKey)
        {
            PrivateKey = privateKey;
        }

        public Transaction Sign(string recipientAddress, decimal value)
        {
            DateTime signDate = DateTime.Now;
            Transaction signedTransaction = Signer.Sign(PrivateKey, recipientAddress, value, signDate);

            return signedTransaction;
        }

        public static SimpleWalet GenerateNewWallet()
        {
            string privateKeyString = new CryptoUtil().GetRandomPrivateKey(10); // must be 16?

            return new SimpleWalet(privateKeyString);
        }

        public string GetAddress()
        {
            string address = Signer.CalculateAddress(PrivateKey);
            return address;
        }


    }
}
