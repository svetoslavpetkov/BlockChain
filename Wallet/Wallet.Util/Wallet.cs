using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;

namespace Wallet.Util
{
    public class Wallet
    {
        public Wallet(string privateKey)
        {
            PrivateKey = privateKey;
        }

        public Transaction Sign(string sender,string receiver, int value)
        {
            throw new NotImplementedException();
        }

        public static Wallet GenerateNew()
        {
            var randomPrivateKeyPair = GenerateRandomKeys();

            BigInteger privateKey = ((ECPrivateKeyParameters)randomPrivateKeyPair.Private).D;

            string privateKeyString = privateKey.ToString(10);

            return new Wallet(privateKeyString);
        }

        public string PrivateKey { get; set; }


        #region HelperMethos

        public static AsymmetricCipherKeyPair GenerateRandomKeys(int keySize = 256)
        {
            ECKeyPairGenerator gen = new ECKeyPairGenerator();
            SecureRandom secureRandom = new SecureRandom();
            KeyGenerationParameters keyGenParam =
                new KeyGenerationParameters(secureRandom, keySize);
            gen.Init(keyGenParam);
            return gen.GenerateKeyPair();
        }

        static readonly X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");

        public static ECPoint GetPublicKeyFromPrivateKey(BigInteger privKey)
        {
            ECPoint pubKey = curve.G.Multiply(privKey).Normalize();
            return pubKey;
        }

        #endregion
    }
}
