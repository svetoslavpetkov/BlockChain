using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace BlockChain.Core
{
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

}
