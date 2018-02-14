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
            var objectForHash = new { Index, Transactions, Difficulty, PreviousBlockHash, CreatedDate,Nonce };
            string json = JsonConvert.SerializeObject(objectForHash);
            string hash = CryptoUtil.CalcSHA256String(json);

            return hash;
        }

        public static Block CreateGenesisBlock(int difficulty)
        {
            DateTime now = DateTime.Now;
            Block genesis = new Block()
            {
                Transactions = new List<Transaction>()
                {
                    new Transaction() { FromAddress="GENESIS" ,ToAddress="a1",Amount=1000,DateCreated = now.AddDays(-10), TransactionHash="GENESIS_010101010101010101" },
                    new Transaction() { FromAddress="GENESIS" ,ToAddress="a2",Amount=1000,DateCreated = now.AddDays(-10), TransactionHash="GENESIS_020202020202020202" }, 
                },
                CreatedDate = DateTime.UtcNow,
                Difficulty = difficulty,
                PreviousBlockHash = string.Empty,
                Index = 1,
                Nonce = 02313,
                MinedBy = string.Empty,
            };

            genesis.BlockHash = genesis.GetHash();

            return genesis;
        }

        public static Block BuildBlockForMiner(int index, List<Transaction> pendingTxs, string prevBlockHash, int difficulty)
        {
           Block block = new Block { Index = index, Transactions = pendingTxs, PreviousBlockHash = prevBlockHash, Difficulty = difficulty };
           block.BlockHash = block.GetHash();
           return block;
        }
    }
}