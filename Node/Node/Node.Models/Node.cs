using System;
using System.Collections.Generic;

namespace Node.Models
{
    public class Node
    {
        public string Name { get; private set; }
        public string About { get; private set; }

        public List<Block> BlockChain { get; private set; }
        public List<Transaction> PendingTransactions { get; private set; }
        public Dictionary<string, Block> MiningJobs { get; private set; }
        public int Difficulty { get; private set; }

        //TODO
        public List<Peer> Peers { get; private set; }
        public List<Balance> Balances { get; private set; }

        public Node()
        {
            BlockChain = new List<Block>();
            PendingTransactions = new List<Transaction>();
            MiningJobs = new Dictionary<string, Block>();
            Peers = new List<Peer>();
            Balances = new List<Balance>();
        }
    }
}
