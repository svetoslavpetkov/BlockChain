using HBitcoin.KeyManagement;
using NBitcoin;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Text;
namespace Wallet.Util.Full
{
    public class Wallet
    {
        private Safe Safe { get; set; }

        public Wallet(string password, string walletFilePath)
        {
            Safe = Safe.Load(password, walletFilePath);
        }

        public string GetAddress(int addressIndex)
        {
            return Safe.GetAddress(addressIndex).ToString();
        }

        public string GetPrivateKey(int addressIndex)
        {
            return Safe.FindPrivateKey(Safe.GetAddress(addressIndex)).ToString();
        }

        private static readonly Network CurrentNetwork = Network.TestNet;

        public string NodeAddress { get; set; }

        public Transaction Sign(string senderAddress, string receiverAddress, int value)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Create new wallet and returns the menmonic
        /// </summary>
        /// <param name="password"></param>
        /// <param name="walletFilePath"></param>
        /// <returns></returns>
        public static string GenerateNew(string password, string walletFilePath)
        {
            Mnemonic mnemonic;
            Safe safe = Safe.Create(out mnemonic, password, walletFilePath, CurrentNetwork);

            return mnemonic.ToString();

            //var randomPrivateKeyPair = GenerateRandomKeys();

            //BigInteger privateKey = ((ECPrivateKeyParameters)randomPrivateKeyPair.Private).D;

            //string privateKeyString = privateKey.ToString(10);

            //return new Wallet(privateKeyString);
        }

        public string GetPublicKeyCompressed(string privateKeyString)
        {
            BigInteger privateKey = new BigInteger(privateKeyString, 10);
            ECPoint pubKey = GetPublicKeyFromPrivateKey(privateKey);

            string pubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            return pubKeyCompressed;
        }

        public string GetAddress(string privateKeyString)
        {
            return CalcRipeMD160(GetPublicKeyCompressed(privateKeyString));
        }

        #region HelperMethos

        private static string CalcRipeMD160(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            RipeMD160Digest digest = new RipeMD160Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);
        }

        public static string BytesToHex(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }

        public static string EncodeECPointHexCompressed(ECPoint point)
        {
            BigInteger x = point.XCoord.ToBigInteger();
            return x.ToString(16) + Convert.ToInt32(!x.TestBit(0));
        }

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
