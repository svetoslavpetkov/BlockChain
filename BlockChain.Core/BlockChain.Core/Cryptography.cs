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

namespace BlockChain.Core
{
    public class CryptographyBase
    {
        protected readonly X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");

        protected byte[] CalcSHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
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
            string publicKey = GetPublicKeyCompressed(privateKey);
            string senderAddress = CalcRipeMD160(publicKey);

            TransactionRaw transactionRaw = new TransactionRaw()
            {
                FromAddress = senderAddress,
                ToAddress = recipientAddress,
                Amount = amount,
                DateCreated = signDate
            };

            string tranJson = JsonConvert.SerializeObject(transactionRaw);
            byte[] tranHash = CalcSHA256(tranJson);

            BigInteger[] tranSignature = SignData(hexPrivateKey, tranHash);
            Transaction signedTransaction = new Transaction()
            {
                FromAddress = transactionRaw.FromAddress,
                ToAddress = transactionRaw.ToAddress,
                Amount = transactionRaw.Amount,
                DateCreated = transactionRaw.DateCreated,

                SenderPublicKey = publicKey,
                Signature = new string[] { tranSignature[0].ToString(16), tranSignature[1].ToString(16) }
            };

            return signedTransaction;
        }

        public string CalculateAddress(string privateKey)
        {
            return CalcRipeMD160(GetPublicKeyCompressed(privateKey));
        }

        private string GetPublicKeyCompressed(string privateKeyString)
        {
            BigInteger privateKey = new BigInteger(privateKeyString, 16);
            Org.BouncyCastle.Math.EC.ECPoint pubKey = GetPublicKeyFromPrivateKey(privateKey);

            string pubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            return pubKeyCompressed;
        }

        private string EncodeECPointHexCompressed(Org.BouncyCastle.Math.EC.ECPoint point)
        {
            var compressedPoint = point.GetEncoded(true);
            BigInteger biInt = new BigInteger(compressedPoint);

            return biInt.ToString(16);
        }

        private Org.BouncyCastle.Math.EC.ECPoint GetPublicKeyFromPrivateKey(BigInteger privKey)
        {
            Org.BouncyCastle.Math.EC.ECPoint pubKey = curve.G.Multiply(privKey).Normalize();
            return pubKey;
        }

        private string CalcRipeMD160(string text)
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

        /// <summary>
        /// Calculates deterministic ECDSA signature (with HMAC-SHA256), based on secp256k1 and RFC-6979.
        /// </summary>
        private BigInteger[] SignData(BigInteger privateKey, byte[] data)
        {
            ECDomainParameters ecSpec = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
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
    }

    public class TransactionValidator : CryptographyBase, ITransactionValidator
    {
        public bool IsValid(Transaction transaction)
        {
            ECDomainParameters ecSpec = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
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
            byte[] tranHash = CalcSHA256(tranJson);

            return signer.VerifySignature(tranHash, pubKey1, pubKey2);
        }

        private Org.BouncyCastle.Math.EC.ECPoint DecodeECPointPublicKey(string input)
        {
            BigInteger bigInt = new BigInteger(input, 16);
            byte[] compressedKey = bigInt.ToByteArray();

            var point = curve.Curve.DecodePoint(compressedKey);
            return point;
        }

    }
}

