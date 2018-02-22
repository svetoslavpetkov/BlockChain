using BlockChain.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Wallet.Util
{
    public interface IBlockChainClient
    {
        ulong GetBalance(string address);
        bool SendTransaction(Transaction transaction);
    }

    public class BlockChainClient : IBlockChainClient
    {
        public string NodeUrl { get; set; } = "http://localhost:5555";

        public ulong GetBalance(string address)
        {
            ulong balance = 0;
            balance = Get<ulong>(NodeUrl +  $"/api/account/{address}/ballance");

            return balance;
        }

        public bool SendTransaction(Transaction transaction)
        {
            return MakePost(NodeUrl + "/api/transaction/new", transaction);
        }

        bool MakePost<T>(string url, T postObject)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string postContent = JsonConvert.SerializeObject(postObject);
                var content = new StringContent(postContent, Encoding.UTF8, "application/json");

                var result = httpClient.PostAsync(url, content).GetAwaiter().GetResult();

                return result.IsSuccessStatusCode;
            }
        }


        T Get<T>(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(url).GetAwaiter().GetResult();
                string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                T responceData = JsonConvert.DeserializeObject<T>(json);

                return responceData;
            }
        }
    }
}
