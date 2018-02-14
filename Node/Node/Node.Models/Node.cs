using BlockChain.Core;
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

        public ConcurrentDictionary<int,Block> BlockChain { get; private set; }
        public ConcurrentBag<Core.Transaction> PendingTransactions { get; private set; }
        public ConcurrentDictionary<string, Block> MiningJobs { get; private set; }
        public int Difficulty { get; private set; }
        private ITransactionValidator TranasctionValidator = new TransactionValidator();

        public ConcurrentBag<Peer> Peers { get; private set; }

        public Node()
        {
            BlockChain = new ConcurrentDictionary<int,Block>();
            PendingTransactions = new ConcurrentBag<Transaction>();
            MiningJobs = new ConcurrentDictionary<string, Block>();
            Peers = new ConcurrentBag<Peer>();
            Difficulty = 5;

            Block genesisBlock = Block.CreateGenesisBlock();
            BlockChain.TryAdd(0,genesisBlock);
        }

        public void AddTransaction(Core.Transaction transaction)
        {
            //check whetehr we know that transaction
            if(PendingTransactions.FirstOrDefault(t => t.TransactionHash == transaction.TransactionHash) != null)
            {
                //should we also notify other perrs for known transactions ?
                return;
            }

            //check whether transaction is already mined
            if(BlockChain.Any(b=> b.Value.Transactions.Any(t=> t.TransactionHash == transaction.TransactionHash)))
            {
                return;
            }

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

        public MiningContext GetMiningJob(string minerAddress, string blockHash)
        {
            MiningContext context = new MiningContext();
            //

            return context;
        }
    }
}
