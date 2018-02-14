using BlockChain.Core;
using Node.Domain.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Core = BlockChain.Core;
using System.Linq;

namespace Node.Domain
{
    public class Node
    {
        public string Name { get; private set; }
        public string About { get; private set; }

        private ConcurrentBag<Block> BlockChain { get; set; }
        private Dictionary<int, string> BlockHashes { get; set; }
        private ConcurrentBag<Core.Transaction> PendingTransactions { get; set; }
        public Dictionary<string, Block> MiningJobs { get; set; }
        public int Difficulty { get; set; }
        public TransactionValidator TransactionValidator { get;set; }
        public ICryptoUtil CryptoUtil { get;set; }

        public List<Peer> Peers { get; private set; }

        public Node()
        {
            BlockChain = new ConcurrentBag<Block>();
            PendingTransactions = new ConcurrentBag<Transaction>();
            MiningJobs = new Dictionary<string, Block>();
            Peers = new List<Peer>();

            Block genesisBlock = Block.CreateGenesisBlock(Difficulty);
            BlockChain.Add(genesisBlock);

            TransactionValidator = new TransactionValidator();
            CryptoUtil = new CryptoUtil();
        }

        public void AddTransaction(Core.Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentException("'transaction' object cannot be null");

            if (string.IsNullOrWhiteSpace(transaction.SenderPublicKey))
                throw new TransactionNotValidException("Sender signature cannot be empty");

            bool isHashValid = TransactionValidator.IsValidateHash(transaction);
            if (!isHashValid)
                throw new TransactionNotValidException("Transaction not Valid! Tranascion is chnaged by middle man");

            bool isVerified = TransactionValidator.IsValid(transaction);
            if(!isVerified)
                throw new TransactionNotValidException("Transaction not Valid! Signutre is not valid");

            string address = TransactionValidator.GetAddress(transaction.SenderPublicKey);
            if (address != transaction.FromAddress)
                throw new TransactionNotValidException("Owner address is not valid.");

            PendingTransactions.Add(transaction);
            BroadcastToPeers(transaction);
        }

        private void BroadcastToPeers(Core.Transaction tx)
        {
            //TODO
        }

        public MiningContext StartMining(string minerAddress)
        {
            Block blockForMine = BuildBlock();
            string prevBlockHash = BlockChain.Last().BlockHash;

            MiningContext context = new MiningContext();
            context.BlockIndex = blockForMine.Index;
            context.BlockHash = blockForMine.BlockHash;
            context.Difficulty = Difficulty;
            context.PrevBlockHash = prevBlockHash;
            context.Timestamp = blockForMine.CreatedDate;

            return context;
        }

        private Block BuildBlock()
        {
            var lastBlock = BlockChain.Last();
            Block tempBlock = Block.BuildBlockForMiner(lastBlock.Index + 1, PendingTransactions.ToList(), lastBlock.BlockHash, Difficulty);

            return tempBlock;
        }
    }
}
