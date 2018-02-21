﻿using BlockChain.Core;
using Node.Domain.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core = BlockChain.Core;

namespace Node.Domain
{
    public class Node
    {
        public Guid Identifier { get; set; } = new Guid();
        public string Name { get; private set; }
        public string About { get; private set; }

        public DateTime Started { get; set; }

        public ConcurrentDictionary<int, Block> BlockChain { get; private set; }
        public ConcurrentBag<Core.Transaction> PendingTransactions { get; private set; }
        public ConcurrentDictionary<string, Block> MiningJobs { get; private set; }
        public int Difficulty { get; private set; }
        private ConcurrentDictionary<string, Block> BlocksInProgress { get; set; }

        public TransactionValidator TransactionValidator { get; set; }


        private IProofOfWork ProofOfWork { get; set; }



        public ICryptoUtil CryptoUtil { get; set; }

        public ConcurrentBag<Peer> Peers { get; private set; }

        public Node()
        {
            BlockChain = new ConcurrentDictionary<int, Block>();
            PendingTransactions = new ConcurrentBag<Transaction>();
            MiningJobs = new ConcurrentDictionary<string, Block>();
            Peers = new ConcurrentBag<Peer>();
            Difficulty = 5;
            BlocksInProgress = new ConcurrentDictionary<string, Block>();

            Block genesisBlock = Block.CreateGenesisBlock(Difficulty);
            BlockChain.TryAdd(0, genesisBlock);

            TransactionValidator = new TransactionValidator();
            CryptoUtil = new CryptoUtil();
            ProofOfWork = new ProofOfWork();
            Started = DateTime.Now;
        }

        public ulong GetBalance(string address)
        {
            ulong balance = CalculateBalance(address);
            return balance;
        }

        public void AddTransaction(Core.Transaction transaction)
        {
            //check whetehr we know that transaction
            if (PendingTransactions.FirstOrDefault(t => t.TransactionHash == transaction.TransactionHash) != null)
            {
                //should we also notify other perrs for known transactions ?
                return;
            }

            //check whether transaction is already mined
            if (BlockChain.Any(b => b.Value.Transactions.Any(t => t.TransactionHash == transaction.TransactionHash)))
            {
                return;
            }

            if (transaction == null)
                throw new ArgumentException("'transaction' object cannot be null");

            if (string.IsNullOrWhiteSpace(transaction.SenderPublicKey))
                throw new TransactionNotValidException("Sender signature cannot be empty");

            bool isHashValid = TransactionValidator.IsValidateHash(transaction);
            if (!isHashValid)
                throw new TransactionNotValidException("Transaction not Valid! Tranascion is chnaged by middle man");

            bool isVerified = TransactionValidator.IsValid(transaction);
            if (!isVerified)
                throw new TransactionNotValidException("Transaction not Valid! Signutre is not valid");

            string address = TransactionValidator.GetAddress(transaction.SenderPublicKey);
            if (address != transaction.FromAddress)
                throw new TransactionNotValidException("Provided address is not valid.");

            PendingTransactions.Add(transaction);
            BroadcastToPeers(transaction);
        }

        public void NonceFound(string minerAddress, int nonce, string hash)
        {
            Block block = BlocksInProgress[minerAddress];
            if (block == null)
                return;

            if (!ProofOfWork.IsProofValid(block.Difficulty, block.Index, block.BlockDataHash, block.PreviousBlockHash, block.CreatedDate, nonce, hash))
            {
                throw new Exception("Invalid proof of work");
            }

            block.BlockMined(nonce, hash, minerAddress);

            foreach (var transaction in block.Transactions)
            {
                decimal balance = CalculateBalance(transaction.FromAddress);
                transaction.TranserSuccessfull = balance >= transaction.Amount;
            }

            int minedTransactionsCount = block.Transactions.Count;

            List<Transaction> notMinedTxs = new List<Transaction>();
            var allTx = PendingTransactions.ToList();

            for (int i = minedTransactionsCount; i < allTx.Count; i++)
                notMinedTxs.Add(allTx[i]);

            PendingTransactions = new ConcurrentBag<Transaction>(notMinedTxs);

            var test = BlockChain.TryAdd(block.Index, block);
            BlocksInProgress[minerAddress] = null;

        }

        private ulong CalculateBalance(string address)
        {
            //Get pur transaction balances
            var addressTransactions = GetTransactions(address);
            ulong balance = 0;
            foreach (var tx in addressTransactions)
            {
                if (tx.FromAddress == address)
                {
                    balance -= tx.Amount;
                    balance -= tx.Fee;
                }

                if (tx.ToAddress == address)
                    balance += tx.Amount;
            }

            //if address is miner winner

            ulong minedBlokcs = BlockChain
                .Where(b => b.Value.MinedBy == address)
                .ToList()
                .Aggregate((ulong)0, (total, nextBlock) => total += nextBlock.Value.BlockReward);

            return balance + minedBlokcs;
        }

        public ICollection<Transaction> GetTransactions(string address, bool includeUncofirmed = false)
        {
            var result = BlockChain.SelectMany(b => b.Value.Transactions).Where(t => t.FromAddress == address || t.ToAddress == address).ToList();

            if (includeUncofirmed)
            {
                result.AddRange(PendingTransactions.Where(t => t.FromAddress == address || t.ToAddress == address).ToList());
            }

            return result;
        }

        private void BroadcastToPeers(Core.Transaction tx)
        {
            //TODO
        }

        public MiningContext GetBlockForMine(string minerAddress)
        {
            Block blockInProgressForMiner = null;
            BlocksInProgress.TryGetValue(minerAddress, out blockInProgressForMiner);

            if (blockInProgressForMiner == null)
            {
                Block blockForMine = BuildBlock();
                MiningContext context = BuildNewMinerJob(blockForMine);
                BlocksInProgress[minerAddress] = blockForMine;

                return context;
            }
            else
            {
                Block lastBlock = BlockChain.Last().Value;
                bool hasNewBlock = lastBlock.Index >= blockInProgressForMiner.Index;
                bool hasNewerTransactions = false;

                // still mining same block
                if (lastBlock.Index == blockInProgressForMiner.Index - 1)
                    hasNewerTransactions = PendingTransactions.Count > blockInProgressForMiner.Transactions.Count;

                if (hasNewBlock || hasNewerTransactions)
                {
                    Block blockForMine = BuildBlock();
                    MiningContext context = BuildNewMinerJob(blockForMine);
                    BlocksInProgress[minerAddress] = blockForMine;

                    return context;
                }

                return BuildNewMinerJob(blockInProgressForMiner);
            }
        }

        private MiningContext BuildNewMinerJob(Block blockForMine)
        {
            string prevBlockHash = BlockChain.Last().Value.BlockDataHash;

            MiningContext context = new MiningContext();
            context.BlockIndex = blockForMine.Index;
            context.BlockHash = blockForMine.BlockDataHash;
            context.Difficulty = Difficulty;
            context.PrevBlockHash = prevBlockHash;
            context.Timestamp = blockForMine.CreatedDate;

            return context;
        }

        private Block BuildBlock()
        {
            var lastBlock = BlockChain.Last().Value;
            Block tempBlock = Block.BuildBlockForMiner(lastBlock.Index + 1, PendingTransactions.ToList(), lastBlock.BlockDataHash, Difficulty);
            //TODO: should transaction mney for the miner be explicitly included ?s
            return tempBlock;
        }
    }
}
