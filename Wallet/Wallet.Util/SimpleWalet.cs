using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Wallet.Util.Core
{
    public class SimpleWalet
    {
        private ITransactionSigner Signer = new TransactionSigner();

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
            var randomPrivateKeyPair = GenerateRandomKeys();

            BigInteger privateKey = ((ECPrivateKeyParameters)randomPrivateKeyPair.Private).D;

            string privateKeyString = privateKey.ToString(10); // must be 16?

            return new SimpleWalet(privateKeyString);
        }

        public string GetAddress()
        {
            string address = Signer.CalculateAddress(PrivateKey);
            return address;
        }

        private static AsymmetricCipherKeyPair GenerateRandomKeys(int keySize = 256)
        {
            ECKeyPairGenerator gen = new ECKeyPairGenerator();
            SecureRandom secureRandom = new SecureRandom();
            KeyGenerationParameters keyGenParam =
                new KeyGenerationParameters(secureRandom, keySize);
            gen.Init(keyGenParam);
            return gen.GenerateKeyPair();
        }
    }
}
