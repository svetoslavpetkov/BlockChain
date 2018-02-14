using BlockChain.Core;
using Node.Domain.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using Core = BlockChain.Core;

namespace Node.Domain
{
    public class Node
    {
        public Guid Identifier { get; set; } = new Guid();
        public string Name { get; private set; }
        public string About { get; private set; }

        public ConcurrentDictionary<int, Block> BlockChain { get; private set; }
        public ConcurrentBag<Core.Transaction> PendingTransactions { get; private set; }
        public ConcurrentDictionary<string, Block> MiningJobs { get; private set; }
        public int Difficulty { get; private set; }
        private ConcurrentDictionary<string, Block> BlocksInProgress { get; set; }

        public TransactionValidator TransactionValidator { get; set; }

        public void NonceFound(string minerAddress, int nonce)
        {
            Block blockForValidate = BlocksInProgress[minerAddress];
            if (blockForValidate == null)
                return;

            // TODO validate nonce by calculating hash

            foreach (var transaction in blockForValidate.Transactions)
            {
                string address = transaction.FromAddress;

                var addressTransactions = BlockChain.SelectMany(b => b.Value.Transactions).Where(t => t.FromAddress == address || t.ToAddress == address).ToList();
                decimal balance = 0;
                foreach (var tx in addressTransactions)
                {
                    if (tx.FromAddress == address)
                        balance -= tx.Amount;

                    if (tx.ToAddress == address)
                        balance += tx.Amount;
                }
            }
        }

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
                BlocksInProgress.TryAdd(minerAddress, blockForMine);

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

                if(hasNewBlock || hasNewerTransactions)
                {
                    Block blockForMine = BuildBlock();
                    MiningContext context = BuildNewMinerJob(blockForMine);
                    BlocksInProgress.TryAdd(minerAddress, blockForMine);

                    return context;
                }
            }

            return null;
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

            return tempBlock;
        }
    }
}
