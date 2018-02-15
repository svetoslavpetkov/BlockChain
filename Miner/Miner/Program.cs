using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Miner
{
    class Program
    {
        static void Main(string[] args)
        {
            string minerAddress = "ohoboho13";
            string nodeAddress = "http://localhost:5555";
            TimeSpan timeLimit = new TimeSpan(0, 0, 5);
            Stopwatch sw = Stopwatch.StartNew();
            BlockInput input = Get<BlockInput>(nodeAddress + "/api/mining/getBockForMine/" + minerAddress);
            while (true)
            {
                sw.Restart();

                Boolean blockFound = false;
                UInt64 nonce = 0;
                string timestamp = input.Timestamp.ToString("o");
                string difficulty = new String('0', input.Difficulty) +
                    new String('9', 64 - input.Difficulty);

                string precomputedData = input.BlockIndex.ToString()
                    + input.BlockHash
                    + input.PrevBlockHash;

                Console.WriteLine($"New job started at : " + DateTime.Now + " for block index " + input.BlockIndex);


                string data;
                string blockHash;
                while (!blockFound && nonce < UInt32.MaxValue)
                {
                    data = precomputedData + timestamp + nonce.ToString();
                    blockHash = ByteArrayToHexString(Sha256(Encoding.UTF8.GetBytes(data)));

                    if (String.CompareOrdinal(blockHash, difficulty) < 0)
                    {
                        MakePost(nodeAddress + "/api/mining/noncefound", new BlockMinedRequest { MinerAddress= minerAddress, Nonce = nonce, Hash =  blockHash });
                        Console.WriteLine($"Block mined. Nonce: {nonce} , Hash: {blockHash}");
                        blockFound = true;
                    }

                    if (blockFound || (nonce % 1000 == 0 && sw.Elapsed >= timeLimit))
                    {
                        sw.Restart();
                        var requestedBlockToMine = Get<BlockInput>(nodeAddress + "/api/mining/getBockForMine/" + minerAddress);
                        if (blockFound || (requestedBlockToMine.BlockHash != input.BlockHash && requestedBlockToMine.BlockIndex != input.BlockIndex))
                        {
                            input = requestedBlockToMine;
                            break;
                        }
                    }
                    nonce++;
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



        public static T Get<T>(string url)
            where T: class
        {            
            using (HttpClient httpClient = new HttpClient())
            {
                //string postContent = JsonConvert.SerializeObject(postObject);
                //svar content = new StringContent(postContent, Encoding.UTF8, "application/json");
                var task = httpClient.GetAsync(url);

                var response = task.GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return JsonConvert.DeserializeObject<T>(json);                    
                }
            }
            return null;
        }



        public static bool MakePost<T>(string url, T postObject)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string postContent = JsonConvert.SerializeObject(postObject);
                var content = new StringContent(postContent, Encoding.UTF8, "application/json");

                var result = httpClient.PostAsync(url, content).GetAwaiter().GetResult();

                return result.IsSuccessStatusCode;
            }
        }

    }
}
