using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using System;
using System.Text;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace BlockChain.Core
{

    public class CryptographyBase
    {
        protected readonly ICryptoUtil CryptoUtil = new CryptoUtil();

        protected string CalcRipeMD160(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            RipeMD160Digest digest = new RipeMD160Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);
        }

        private string BytesToHex(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }
    }

    public interface ITransactionSigner
    {
        Transaction Sign(string privateKey, string recipientAddress, decimal amount, DateTime signDate);
        string CalculateAddress(string privateKey);
    }

    public class TransactionSigner : CryptographyBase, ITransactionSigner
    {
        public Transaction Sign(string privateKey, string recipientAddress, decimal amount, DateTime signDate)
        {
            BigInteger hexPrivateKey = new BigInteger(privateKey, 16);
            string publicKey = CryptoUtil.GetPublicKeyCompressed(privateKey);
            string senderAddress = CalcRipeMD160(publicKey);

            TransactionRaw transactionRaw = new TransactionRaw()
            {
                FromAddress = senderAddress,
                ToAddress = recipientAddress,
                Amount = amount,
                DateCreated = signDate
            };

            string tranJson = JsonConvert.SerializeObject(transactionRaw);
            byte[] tranHash = CryptoUtil.CalcSHA256(tranJson);

            BigInteger[] tranSignature = SignData(hexPrivateKey, tranHash);
            string txhash = CryptoUtil.CalcSHA256String(tranJson);

            Transaction signedTransaction = new Transaction()
            {
                FromAddress = transactionRaw.FromAddress,
                ToAddress = transactionRaw.ToAddress,
                Amount = transactionRaw.Amount,
                DateCreated = transactionRaw.DateCreated,
                TransactionHash = txhash,
                SenderPublicKey = publicKey,
                Signature = new string[] { tranSignature[0].ToString(16), tranSignature[1].ToString(16) }
            };

            return signedTransaction;
        }

        public string CalculateAddress(string privateKey)
        {
            return CalcRipeMD160(CryptoUtil.GetPublicKeyCompressed(privateKey));
        }

        /// <summary>
        /// Calculates deterministic ECDSA signature (with HMAC-SHA256), based on secp256k1 and RFC-6979.
        /// </summary>
        private BigInteger[] SignData(BigInteger privateKey, byte[] data)
        {
            ECDomainParameters ecSpec = new ECDomainParameters(CryptoUtil.Curve.Curve, CryptoUtil.Curve.G, CryptoUtil.Curve.N, CryptoUtil.Curve.H);
            ECPrivateKeyParameters keyParameters = new ECPrivateKeyParameters(privateKey, ecSpec);
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            ECDsaSigner signer = new ECDsaSigner(kCalculator);
            signer.Init(true, keyParameters);
            BigInteger[] signature = signer.GenerateSignature(data);
            return signature;
        }
    }

    public interface ITransactionValidator
    {
        bool IsValid(Transaction transaction);
        bool IsValidateHash(Transaction transaction);
        string GetAddress(string publicKey);
    }

    public class TransactionValidator : CryptographyBase, ITransactionValidator
    {
        public bool IsValid(Transaction transaction)
        {
            ECDomainParameters ecSpec = new ECDomainParameters(CryptoUtil.Curve.Curve, CryptoUtil.Curve.G, CryptoUtil.Curve.N, CryptoUtil.Curve.H);
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());

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
            byte[] tranHash = CryptoUtil.CalcSHA256(tranJson);

            return signer.VerifySignature(tranHash, pubKey1, pubKey2);
        }

        public bool IsValidateHash(Transaction transaction)
        {
            TransactionRaw txRaw = new TransactionRaw();
            txRaw.ToAddress = transaction.ToAddress;
            txRaw.FromAddress = transaction.FromAddress;
            txRaw.Amount = transaction.Amount;
            txRaw.DateCreated = transaction.DateCreated;

            string jsonTx = JsonConvert.SerializeObject(txRaw);
            string txHash = CryptoUtil.CalcSHA256String(jsonTx);

            return transaction.TransactionHash != txHash;
        }

        public string GetAddress(string publicKey)
        {
            string address = CalcRipeMD160(publicKey);
            return address;
        }

        private Org.BouncyCastle.Math.EC.ECPoint DecodeECPointPublicKey(string input)
        {
            BigInteger bigInt = new BigInteger(input, 16);
            byte[] compressedKey = bigInt.ToByteArray();

            var point = CryptoUtil.Curve.Curve.DecodePoint(compressedKey);
            return point;
        }
    }

    public interface ICryptoUtil
    {
        X9ECParameters Curve { get; }

        AsymmetricCipherKeyPair GenerateRandomKeys(int keySize = 256);

        string GetRandomPrivateKey();

        byte[] CalcSHA256(string text);

        string CalcSHA256String(string text);

        string GetPublicKeyCompressed(string privateKeyString);
    }

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