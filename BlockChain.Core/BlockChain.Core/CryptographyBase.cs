using Org.BouncyCastle.Crypto.Digests;
using System.Linq;
using System.Text;

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

}
