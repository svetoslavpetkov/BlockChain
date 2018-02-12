using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wallet.Util.Core
{
    public class SimpleWalet
    {
        static readonly X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");


        public string PrivateKey { get; set; }
        public int PrivateKeyRadix { get; set; }

        public string PublicKey { get; set; }

        public Org.BouncyCastle.Math.EC.ECPoint PublicKeyPoint { get; set; }


        public SimpleWalet(string privateKey, int radix)
        {
            PrivateKey = privateKey;
            PrivateKeyRadix = radix;

            PublicKey = GetPublicKeyCompressed(privateKey);

            PublicKeyPoint = GetPublicKeyFromPrivateKey(new BigInteger(PrivateKey, radix));
        }


        public Transaction Sign(string recipientAddress, decimal value, DateTime signDate)
        {
            BigInteger privateKey = new BigInteger(PrivateKey, PrivateKeyRadix);           
            string senderAddress = CalcRipeMD160(PublicKey);

            TransactionRaw transactionRaw = new TransactionRaw()
            {
                FromAddress = senderAddress,
                ToAddress = recipientAddress,
                Amount = value,
                DateCreated = signDate
            };

            string tranJson = JsonConvert.SerializeObject(transactionRaw);
            byte[] tranHash = CalcSHA256(tranJson);

            BigInteger[] tranSignature = SignData(privateKey, tranHash);


            return new Transaction() {
                FromAddress = transactionRaw.FromAddress,
                ToAddress = transactionRaw.ToAddress,
                Amount = transactionRaw.Amount,
                DateCreated = transactionRaw.DateCreated,

                SenderPublicKey = PublicKey,
                Signature = new string[] { tranSignature[0].ToString(16),tranSignature[1].ToString(16) }                
            };
        }


        public bool IsValid(Transaction transaction)
        {
            ECDomainParameters ecSpec = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());

            //BigInteger pubKey = new BigInteger(transaction.SenderPublicKey, 16);


            //byte[] xEnc = X9IntegerConverter.IntegerToBytes(pubKey, X9IntegerConverter.GetByteLength(curve.Curve));
            //byte[] compEncoding = new byte[xEnc.Length + 1];
            //compEncoding[0] = (byte)(0x02 + (recid & 1));
            //xEnc.CopyTo(compEncoding, 1);

            //var publicPOINTTTT =  curve.Curve.DecodePoint(compEncoding);

            var point = DecodeECPointPublicKey(transaction.SenderPublicKey);

            ECPublicKeyParameters keyParameters = new ECPublicKeyParameters(point, ecSpec);


            ECDsaSigner signer = new ECDsaSigner(kCalculator);
            signer.Init(false, keyParameters);

            var pubKey1 = new BigInteger(transaction.Signature[0], 16);
            var pubKey2 = new BigInteger(transaction.Signature[1], 16);

            TransactionRaw transactionRaw = new TransactionRaw()
            {
                FromAddress = transaction.FromAddress,
                ToAddress = transaction.ToAddress,
                Amount = transaction.Amount,
                DateCreated = transaction.DateCreated
            };

            string tranJson = JsonConvert.SerializeObject(transactionRaw);
            byte[] tranHash = CalcSHA256(tranJson);


           return signer.VerifySignature(tranHash, pubKey1, pubKey2);
        }

        private static byte[] CalcSHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }

        /// <summary>
        /// Calculates deterministic ECDSA signature (with HMAC-SHA256), based on secp256k1 and RFC-6979.
        /// </summary>
        private static BigInteger[] SignData(BigInteger privateKey, byte[] data)
        {
            ECDomainParameters ecSpec = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            ECPrivateKeyParameters keyParameters = new ECPrivateKeyParameters(privateKey, ecSpec);
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            ECDsaSigner signer = new ECDsaSigner(kCalculator);
            signer.Init(true, keyParameters);
            BigInteger[] signature = signer.GenerateSignature(data);
            return signature;
        }

        public static SimpleWalet GenerateNewWallet()
        {
            var randomPrivateKeyPair = GenerateRandomKeys();

            BigInteger privateKey = ((ECPrivateKeyParameters)randomPrivateKeyPair.Private).D;

            string privateKeyString = privateKey.ToString(10);

            return new SimpleWalet(privateKeyString,10);
        }



        public string GetPublicKeyCompressed(string privateKeyString)
        {
            BigInteger privateKey = new BigInteger(privateKeyString, PrivateKeyRadix);
            Org.BouncyCastle.Math.EC.ECPoint pubKey = GetPublicKeyFromPrivateKey(privateKey);

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

        public static string EncodeECPointHexCompressed(Org.BouncyCastle.Math.EC.ECPoint point)
        {
            /*
            BigInteger x = point.XCoord.ToBigInteger();
            return x.ToString(16) + Convert.ToInt32(!x.TestBit(0));*/

            var compressedPoint = point.GetEncoded(true);
            BigInteger biInt = new BigInteger(compressedPoint);

            return biInt.ToString(16);
        }

        public static Org.BouncyCastle.Math.EC.ECPoint DecodeECPointPublicKey(string input)
        {
            // int p = 256, a = 0, b = 7;
            //bool isOdd = input[16] == '1';
            //BigInteger pointInt = new BigInteger()
            BigInteger bigInt = new BigInteger(input, 16);
           byte[] compressedKey = bigInt.ToByteArray();

            var point = curve.Curve.DecodePoint(compressedKey);
            return point;

            //curve.Curve.DecodePoint();
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


        public static Org.BouncyCastle.Math.EC.ECPoint GetPublicKeyFromPrivateKey(BigInteger privKey)
        {
            Org.BouncyCastle.Math.EC.ECPoint pubKey = curve.G.Multiply(privKey).Normalize();
            return pubKey;
        }

        #endregion
    }
}
