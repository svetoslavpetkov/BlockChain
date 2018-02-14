using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using System;

namespace BlockChain.Core
{
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
}
