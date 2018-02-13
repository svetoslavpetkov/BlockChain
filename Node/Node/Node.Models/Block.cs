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
            return new Block()
            {
                Transactions = new List<Transaction>()
                {
                    new Transaction("from", "to",10000,DateTime.UtcNow, "PubKey", new string[2] { "x","y" })
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