using BlockChain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain
{
    public class Block
    {
        public int Index { get; private set; }
        public List<Transaction> Transactions { get; private set; }
        public int Difficulty { get; private set; }
        public string PreviousBlockHash { get; private set; }
        public string MinedBy { get; private set; }
        public string BlockHash { get; private set; }

        public int Nonce { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public Block()
        {
            Transactions = new List<Transaction>();
        }

        public static Block CreateGenesisBlock()
        {
            DateTime now = DateTime.Now;
            return new Block()
            {
                Transactions = new List<Transaction>()
                {
                    new Transaction() { ToAddress="a1",Amount=1000,DateCreated = now.AddDays(-10) },
                    new Transaction() { ToAddress="a2",Amount=1000,DateCreated = now.AddDays(-10) },
                },
                CreatedDate = DateTime.UtcNow,
                Difficulty = 1,
                PreviousBlockHash = string.Empty,
                Index = 0,
                Nonce = 12313,
                MinedBy = string.Empty,
            };
        }
    }
}