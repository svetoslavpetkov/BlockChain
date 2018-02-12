using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Miner
{
    class Program
    {
        static void Main(string[] args)
        {
            string minerAddress = "localhost:8080";

            var input = Get<BlockInput>(minerAddress + "//getBlock");

            Boolean blockFound = false;
            UInt64 nonce = 0;
            string timestamp = input.Item2.Timestamp.ToString("o");
            string difficulty = new String('0', input.Item2.Difficulty) +
                new String('9', 64 - input.Item2.Difficulty);

            string precomputedData = input.Item2.BlockIndex.ToString()
                + input.Item2.BlockData
                + input.Item2.PrevBlockHash;

            string data;
            string blockHash;
            while (!blockFound && nonce < UInt32.MaxValue)
            {
                data = precomputedData + timestamp + nonce.ToString();
                blockHash = ByteArrayToHexString(Sha256(Encoding.UTF8.GetBytes(data)));

                if (String.CompareOrdinal(blockHash, difficulty) < 0)
                {

                }
            }

        }


        public static byte[] Sha256(byte[] array)
        {
            SHA256Managed hashstring = new SHA256Managed();
            return hashstring.ComputeHash(array);
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



        public static Tuple<HttpStatusCode, T> Get<T>(string url)
        {
            var statusCode = HttpStatusCode.RequestTimeout;

            // Create a request to Node   
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 3000;
            request.ContentType = "application/json; charset=utf-8";
            using (var response = request.GetResponse())
            {
                statusCode = ((HttpWebResponse)response).StatusCode;

                using (Stream dataStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        string responseFromNode = reader.ReadToEnd();
                        return new Tuple<HttpStatusCode, T>(statusCode, JsonConvert.DeserializeObject<T>(responseFromNode));
                    }
                }
            }

        }
    }
}
