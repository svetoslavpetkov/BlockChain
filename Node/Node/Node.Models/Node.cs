using BlockChain.Core;
using Node.Domain.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Core = BlockChain.Core;

namespace Node.Domain
{
    public class Node
    {
        public string Name { get; private set; }
        public string About { get; private set; }

        public ConcurrentBag<Block> BlockChain { get; private set; }
        public ConcurrentBag<Core.Transaction> PendingTransactions { get; private set; }
        public Dictionary<string, Block> MiningJobs { get; private set; }
        public int Difficulty { get; private set; }
        private ITransactionValidator TranasctionValidator = new TransactionValidator();

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

        public void AddTransaction(Core.Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentException("'transaction' object cannot be null");

            if (string.IsNullOrWhiteSpace(transaction.SenderPublicKey))
                throw new TransactionNotValidException("Sender signature cannot be empty");

            bool isHashValid = TranasctionValidator.IsValidateHash(transaction);
            if (!isHashValid)
                throw new TransactionNotValidException("Transaction not Valid! Tranascion is chnaged by middle man");

            bool isVerified = TranasctionValidator.IsValid(transaction);
            if(!isVerified)
                throw new TransactionNotValidException("Transaction not Valid! Signutre is not valid");

            string address = TranasctionValidator.GetAddress(transaction.SenderPublicKey);
            if (address != transaction.FromAddress)
                throw new TransactionNotValidException("Provided address is not valid.");

            PendingTransactions.Add(transaction);
            BroadcastToPeers(transaction);
        }

        private void BroadcastToPeers(Core.Transaction tx)
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
