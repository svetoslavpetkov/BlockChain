using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Core
{   
    public class ProofOfWork : IProofOfWork
    {
        public string PrecomputeData(int blockIndex, string blockDataHash, string prevBlockHash, DateTime timestamp )
        {
            return blockIndex.ToString()
                    + blockDataHash
                    + prevBlockHash
                    + timestamp.ToString("o");
        }

        public string Compute(int blockIndex, string blockDataHash, string prevBlockHash, DateTime timestamp, int nonce)
        {
            return Compute(PrecomputeData(blockIndex, blockDataHash, prevBlockHash, timestamp), nonce);
        }

        public string Compute(string precomputedData, int nonce)
        {
            string data = precomputedData + nonce.ToString();
            return ByteArrayToHexString(Sha256(Encoding.UTF8.GetBytes(data)));
        }


        public bool IsProofValid(int difficulty,int blockIndex, string blockDataHash, string prevBlockHash, DateTime timestamp, int nonce, string blockHash)
        {
            string computed = Compute(blockIndex, blockDataHash, prevBlockHash, timestamp, nonce);
            if (computed != blockHash)
            {
                return false;
            }

            return IsProofValid(computed, difficulty);
        }

        public bool IsProofValid(string blockHash, int difficulty)
        {
            string difficultyComparer = new String('0', difficulty) +
                    new String('9', 64 - difficulty);

            return String.CompareOrdinal(blockHash, difficultyComparer) < 0;
        }




        private static byte[] Sha256(byte[] array)
        {
            SHA256Managed hashstring = new SHA256Managed();
            return hashstring.ComputeHash(array);
        }

        private static string ByteArrayToHexString(byte[] bytes)
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
    }
}
