using BlockChain.Core;
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
        private static readonly string defaultMinerAddress = "fb01e952e46e641ff3c74616541e292a0c11d455";
        private static readonly string nodeAddress = "http://localhost:5555";
        private static readonly TimeSpan timeLimit = new TimeSpan(0, 0, 5);
        private static readonly TimeSpan retryDelay = new TimeSpan(0, 0, 5);

        static void Main(string[] args)
        {
            IProofOfWork proofOfWork = new ProofOfWork();
            ICryptoUtil cryptoUtil = new CryptoUtil();
            string minerAddress = defaultMinerAddress;
            if (args.Length > 0)
            {
                if(cryptoUtil.IsAddressValid(args[0]))
                {
                    minerAddress = args[0];
                }
                else
                {
                    Output.WriteError($"Prvided address is invalid: {args[0]}. Fallback to default address: {defaultMinerAddress}");
                }
            }

            Console.WriteLine($"Statring mining for {minerAddress}");

            Stopwatch sw = Stopwatch.StartNew();
            BlockInput input = Get<BlockInput>(nodeAddress + "/api/mining/getBockForMine/" + minerAddress);

            while (true)
            {
                sw.Restart();

                Boolean blockFound = false;
                int nonce = 0;

                string precomputedData = proofOfWork.PrecomputeData(input.BlockIndex, input.BlockHash, input.PrevBlockHash, input.Timestamp);

                Console.WriteLine($"New job started at : " + DateTime.Now + " for block index " + input.BlockIndex);


                string data;
                string blockHash;
                while (!blockFound && nonce < int.MaxValue)
                {                    
                    blockHash = proofOfWork.Compute(precomputedData,nonce);

                    if (proofOfWork.IsProofValid(blockHash,input.Difficulty))
                    {
                        var blockFoundResult  = MakePost(nodeAddress + "/api/mining/noncefound", new BlockMinedRequest { MinerAddress= minerAddress, Nonce = nonce, Hash =  blockHash });
                        if (blockFoundResult)
                        {
                            Output.WriteSuccess($"Block mined. Nonce: {nonce} , Hash: {blockHash}");
                        }
                        else
                        {
                            Output.WriteError("Block mined, but not accepted :(");
                        }
                        blockFound = true;
                    }

                    if (blockFound || (nonce % 1000 == 0 && sw.Elapsed >= timeLimit))
                    {
                        var requestedBlockToMine = Get<BlockInput>(nodeAddress + "/api/mining/getBockForMine/" + minerAddress);
                        if (blockFound || requestedBlockToMine.BlockHash != input.BlockHash || requestedBlockToMine.BlockIndex != input.BlockIndex)
                        {
                            input = requestedBlockToMine;
                            break;
                        }
                        sw.Restart();
                    }
                    nonce++;
                }
            }
        }

        public static T Get<T>(string url)
            where T: class
        {
            while (true)
            {
                try
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
                catch (Exception ex)
                {
                    Output.WriteError($"Cannot connect to node. Trying after {retryDelay.TotalSeconds} seconds");
                    System.Threading.Thread.Sleep(retryDelay);
                }
            }
        }



        public static bool MakePost<T>(string url, T postObject)
        {
            while (true)
            {
                try
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        string postContent = JsonConvert.SerializeObject(postObject);
                        var content = new StringContent(postContent, Encoding.UTF8, "application/json");

                        var result = httpClient.PostAsync(url, content).GetAwaiter().GetResult();

                        return result.IsSuccessStatusCode;
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteError($"Cannot connect to node. Trying after {retryDelay.TotalSeconds} seconds");
                    System.Threading.Thread.Sleep(retryDelay);
                }
            }
        }

    }
}
