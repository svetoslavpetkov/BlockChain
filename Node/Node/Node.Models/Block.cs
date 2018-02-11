using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Models
{
    public class Block
    {
        public int Index { get; private set; }
        public List<Transaction> Transactions { get; private set; }
        public int Difficulty { get; private set; }
        public string PreviousBlockHash { get; private set; }
        public string MinedBy { get; private set; }
        public string BlockDataHash { get; private set; }
        public string BlockHash { get; private set; }

        public int Nonce { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public Block()
        {
            Transactions = new List<Transaction>();
        }
    }
}