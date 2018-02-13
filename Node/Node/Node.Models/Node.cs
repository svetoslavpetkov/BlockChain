using Node.Domain.ApiModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Node.Domain
{
    public class Node
    {
        public string Name { get; private set; }
        public string About { get; private set; }

        public ConcurrentBag<Block> BlockChain { get; private set; }
        public ConcurrentBag<Transaction> PendingTransactions { get; private set; }
        public Dictionary<string, Block> MiningJobs { get; private set; }
        public int Difficulty { get; private set; }

        //TODO
        public List<Peer> Peers { get; private set; }

        public Node()
        {
            BlockChain = new ConcurrentBag<Block>();
            PendingTransactions = new ConcurrentBag<Transaction>();
            MiningJobs = new Dictionary<string, Block>();
            Peers = new List<Peer>();

            Block genesisBlock = Block.CreateGenesisBlock();
            BlockChain.Add(genesisBlock);
        }

        public void AddTransaction(TransactionApiModel transaction)
        {
            if (transaction == null)
                throw new ArgumentException("'transaction' object cannot be null");

            Transaction tx = transaction.Convert();
            tx.Validate();
            tx.VerifySenderSignature();
            PendingTransactions.Add(tx);
            BroadcastToPeers(tx);
        }

        private void BroadcastToPeers(Transaction tx)
        {
            //TODO
        }

        public MiningContext StartMining(string minerAddress)
        {
            MiningContext context = new MiningContext();
            return context;
        }
    }
}
