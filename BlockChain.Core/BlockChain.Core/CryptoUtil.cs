using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace BlockChain.Core
{
    public class CryptoUtil : ICryptoUtil
    {
        public readonly X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");

        public X9ECParameters Curve { get { return curve; } }

        public AsymmetricCipherKeyPair GenerateRandomKeys(int keySize = 256)
        {
            ECKeyPairGenerator gen = new ECKeyPairGenerator();
            SecureRandom secureRandom = new SecureRandom();
            KeyGenerationParameters keyGenParam =
                new KeyGenerationParameters(secureRandom, keySize);
            gen.Init(keyGenParam);
            return gen.GenerateKeyPair();
        }

        public string GetRandomPrivateKey()
        {
            var randomPrivateKeyPair = GenerateRandomKeys();

            BigInteger privateKey = ((ECPrivateKeyParameters)randomPrivateKeyPair.Private).D;

            return privateKey.ToString(16);
        }

        public string RecoverPrivateKey(string seed)
        {
            var sha = CalcSHA256(seed);
            BigInteger privateKey = new BigInteger(sha);

            return privateKey.ToString(16);
        }

        public byte[] CalcSHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }

        public string CalcSHA256String(string text)
        {
            byte[] bytes = CalcSHA256(text);
            StringBuilder hash = new StringBuilder();

            foreach (byte b in bytes)
                hash.AppendFormat("{0:X2}", b);

            return hash.ToString();
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            string hexAlphabet = "0123456789ABCDEF";

            foreach (byte b in bytes)
            {
                result.Append(hexAlphabet[(int)(b >> 4)]);
                result.Append(hexAlphabet[(int)(b & 0x0F)]);
            }

            return result.ToString();
        }




        public string GetPublicKeyCompressed(string privateKeyString)
        {
            BigInteger privateKey = new BigInteger(privateKeyString, 16);
            Org.BouncyCastle.Math.EC.ECPoint pubKey = GetPublicKeyFromPrivateKey(privateKey);

            string pubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            return pubKeyCompressed;
        }

        protected Org.BouncyCastle.Math.EC.ECPoint GetPublicKeyFromPrivateKey(BigInteger privKey)
        {
            Org.BouncyCastle.Math.EC.ECPoint pubKey = Curve.G.Multiply(privKey).Normalize();
            return pubKey;
        }

        protected string EncodeECPointHexCompressed(Org.BouncyCastle.Math.EC.ECPoint point)
        {
            var compressedPoint = point.GetEncoded(true);
            BigInteger biInt = new BigInteger(compressedPoint);

            return biInt.ToString(16);
        }
    }
}