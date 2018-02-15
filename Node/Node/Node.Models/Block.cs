using BlockChain.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Node.Domain
{
    public class Block
    {
        public int Index { get; private set; }
        public IReadOnlyCollection<Transaction> Transactions { get; private set; }
        public int Difficulty { get; private set; }
        public string PreviousBlockHash { get; private set; }
        public string MinedBy { get; private set; }
        public string BlockDataHash { get; private set; }
        /// <summary>
        /// Used to store hash calculated from miner    
        /// </summary>
        public string BlockHash { get; private set; }

        public int Nonce { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public ICryptoUtil CryptoUtil { get; set; }

        public Block()
        {
            Transactions = new  List<Transaction>();
            CryptoUtil = new CryptoUtil();
        }

        private string GetHash()
        {
            var objectForHash = new { Index, Transactions, Difficulty, PreviousBlockHash, CreatedDate };
            string json = JsonConvert.SerializeObject(objectForHash);
            string hash = CryptoUtil.CalcSHA256String(json);

            return hash;
        }

        public void BlockMined(int nonce, string hash, string minerAddress)
        {
            Nonce = Nonce;
            BlockHash = hash;
            MinedBy = minerAddress;
        }

        public static Block CreateGenesisBlock(int difficulty)
        {
            DateTime now = DateTime.Now;
            Block genesis = new Block()
            {
                Transactions = new List<Transaction>()
                {
                    new Transaction() { FromAddress="GENESIS" ,ToAddress="f511e2a83e2e05b2511b37da9a9d5736750cf44a",Amount=10000,DateCreated = now.AddDays(-10), TransactionHash="GENESIS_010101010101010101" },
                    new Transaction() { FromAddress="GENESIS" ,ToAddress="4d95c60e34fdc128e007492ed1f40a6c95457bff",Amount=10000,DateCreated = now.AddDays(-10), TransactionHash="GENESIS_020202020202020202" }, 
                },
                CreatedDate = DateTime.UtcNow,
                Difficulty = difficulty,
                PreviousBlockHash = string.Empty,
                Index = 0,
                Nonce = 02313,
                MinedBy = string.Empty,
            };

            genesis.BlockDataHash = genesis.GetHash();

            return genesis;
        }

        public static Block BuildBlockForMiner(int index, List<Transaction> pendingTxs, string prevBlockHash, int difficulty)
        {
           Block block = new Block { Index = index, Transactions = pendingTxs, PreviousBlockHash = prevBlockHash, Difficulty = difficulty };
           block.BlockDataHash = block.GetHash();
           return block;
        }
    }
}