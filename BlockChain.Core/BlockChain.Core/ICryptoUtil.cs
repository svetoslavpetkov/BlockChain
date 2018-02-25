using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;

namespace BlockChain.Core
{
    public interface ICryptoUtil
    {
        X9ECParameters Curve { get; }

        AsymmetricCipherKeyPair GenerateRandomKeys(int keySize = 256);

        string GetRandomPrivateKey();

        byte[] CalcSHA256(string text);

        string CalcSHA256String(string text);

        string GetPublicKeyCompressed(string privateKeyString);

        string RecoverPrivateKey(string seed);

        string GetHexString(byte[] bytes);

        bool IsAddressValid(string address);
    }
}
