using BlockChain.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core = BlockChain.Core;

namespace Node.Domain
{
    public class Node
    {
        internal NodeInfo NodeInfo { get; private set; }
        internal DateTime Started { get; set; }
        internal ConcurrentDictionary<int, Block> BlockChain { get; private set; }
        internal ConcurrentBag<Core.Transaction> PendingTransactions { get; private set; }
        private ConcurrentDictionary<string, Block> MiningJobs { get; set; }
        internal int Difficulty { get; private set; }
        internal ConcurrentDictionary<string, Block> BlocksInProgress { get; set; }
        internal ConcurrentBag<Peer> Peers { get; private set; }

        private ITransactionValidator TransactionValidator { get; set; }
        private INodeSynchornizator NodeSynchornizator { get; set; }
        private IProofOfWork ProofOfWork { get; set; }
        private ICryptoUtil CryptoUtil { get; set; }

        internal Block LastBlock
        {
            get
            {
                return BlockChain.Last().Value;
            }
        }

        public Node(INodeSynchornizator nodeSynchornizator, IProofOfWork proofOfWork,
            ICryptoUtil cryptoUtil, ITransactionValidator transactionValidator, NodeInfo nodeInfo)
        {
            NodeInfo = nodeInfo;
            BlockChain = new ConcurrentDictionary<int, Block>();
            PendingTransactions = new ConcurrentBag<Transaction>();
            MiningJobs = new ConcurrentDictionary<string, Block>();
            Peers = new ConcurrentBag<Peer>();
            Difficulty = 5;
            BlocksInProgress = new ConcurrentDictionary<string, Block>();

            NodeSynchornizator = nodeSynchornizator;
            TransactionValidator = transactionValidator;
            CryptoUtil = cryptoUtil;
            ProofOfWork = proofOfWork;

            Block genesisBlock = Block.CreateGenesisBlock(Difficulty);
            BlockChain.TryAdd(0, genesisBlock);

            Started = DateTime.Now;

           var missedBlocks =  NodeSynchornizator.SyncBlocks();

            foreach (var b in missedBlocks)
            {
                RevalidateBlock(b);
                BlockChain.TryAdd(b.Index, b);
            }
        }

        private void ValidateTransaction(Transaction transaction)
        {
            if (!CryptoUtil.IsAddressValid(transaction.FromAddress))
                throw new AddressNotValidException($"{transaction.FromAddress} is not valid address");

            if (!CryptoUtil.IsAddressValid(transaction.ToAddress))
                throw new AddressNotValidException($"{transaction.ToAddress} is not valid address");

            if (transaction == null)
                throw new ArgumentException("'transaction' object cannot be null");

            //check whetehr we know that transaction
            if (PendingTransactions.FirstOrDefault(t => t.TransactionHash == transaction.TransactionHash) != null)
                return;

            //check whether transaction is already mined
            if (BlockChain.Any(b => b.Value.Transactions.Any(t => t.TransactionHash == transaction.TransactionHash)))
                return;

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
        }

        public void AddTransaction(Transaction transaction)
        {
            ValidateTransaction(transaction);

            ulong senderBalance = CalculateBalance(transaction.FromAddress, true, true);
            transaction.TranserSuccessfull = senderBalance >= transaction.Amount;

            PendingTransactions.Add(transaction);

            if (!transaction.TranserSuccessfull)
                throw new BalanceNotEnoughException($"Address '{transaction.FromAddress}' trying to send {transaction.Amount.GetFormattedTokens()}, but balance is {senderBalance.GetFormattedTokens()}");
        }

        public void NonceFound(string minerAddress, int nonce, string hash)
        {
            Block block = BlocksInProgress[minerAddress];
            if (block == null)
                return;

            if (!CryptoUtil.IsAddressValid(minerAddress))
                throw new AddressNotValidException($"Miner address '{minerAddress}' is not valid");

            ValidateBlockHash(block, nonce, hash);
            block.BlockMined(nonce, hash, minerAddress);

            int minedTransactionsCount = block.Transactions.Count;

            List<Transaction> notMinedTxs = new List<Transaction>();
            var allTx = PendingTransactions.ToList();

            for (int i = minedTransactionsCount; i < allTx.Count; i++)
                notMinedTxs.Add(allTx[i]);

            PendingTransactions = new ConcurrentBag<Transaction>(notMinedTxs);

            BlockChain.TryAdd(block.Index, block);
            BlocksInProgress[minerAddress] = null;
            NodeSynchornizator.BroadcastBlock(block);
        }

        private void ValidateBlockHash(Block block, int nonce, string hash)
        {
            if (!ProofOfWork.IsProofValid(block.Difficulty, block.Index, block.BlockDataHash, block.PreviousBlockHash, block.CreatedDate, nonce, hash))
            {
                throw new Exception("Invalid proof of work");
            }
        }

        public ulong GetBalance(string address)
        {
            return CalculateBalance(address, false, true);
        }

        private ulong CalculateBalance(string address, bool includeUncomfirmed = false,
            bool onlySuccessful = false)
        {
            //Get transaction balances
            var addressTransactions = GetTransactions(address, includeUncomfirmed, onlySuccessful);
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

        internal ICollection<Transaction> GetTransactions(string address, bool includeUncofirmed = false, bool onlySuccessful = false)
        {
            var query = BlockChain.SelectMany(b => b.Value.Transactions).Where(t => t.FromAddress == address || t.ToAddress == address);

            if (onlySuccessful)
            {
                query = query.Where(t => t.TranserSuccessfull);
            }

            var result = query.ToList();

            if (includeUncofirmed)
            {
                result.AddRange(PendingTransactions
                    .Where(t => (t.FromAddress == address || t.ToAddress == address)
                            && (!onlySuccessful || t.TranserSuccessfull))
                    .ToList());
            }

            return result;
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
                bool hasNewBlock = LastBlock.Index >= blockInProgressForMiner.Index;
                bool hasNewerTransactions = false;

                // still mining same block
                if (LastBlock.Index == blockInProgressForMiner.Index - 1)
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
            string prevBlockHash = LastBlock.BlockDataHash;

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

        public void AttachBroadcastedBlock(BlockSyncApiModel block, string nodeAddress)
        {
            bool isPastBlock = block.Index <= LastBlock.Index;
            if (isPastBlock)
                return;

            Block minedBlock = Block.ReCreateBlock(block);
            ValidateBlockHash(minedBlock, minedBlock.Nonce, minedBlock.BlockHash);

            // replace blockchain if another blockain is longer
            int nodeDifference = minedBlock.Index - BlockChain.Count;
            if (nodeDifference >= 6)
            {
                int startIndex = minedBlock.Index - nodeDifference;
                List<Block> forkedBlocks = NodeSynchornizator.GetBlocksForSync(nodeAddress);

                foreach (var bl in forkedBlocks)
                {
                    RevalidateBlock(bl);
                    BlockChain.AddOrUpdate(bl.Index, bl, (index, curBlock) => { return bl; });
                }

                List<string> blockTxs = forkedBlocks.SelectMany(b => b.Transactions).Select(t => t.TransactionHash).ToList();

                PendingTransactions = new ConcurrentBag<Transaction>(PendingTransactions.
                    Where(t => !blockTxs.Contains(t.TransactionHash)));
            }
            else
            {
                bool isFutureBlock = LastBlock.BlockHash != block.PreviousBlockHash;
                if (isFutureBlock)
                    return;

                RevalidateBlock(minedBlock);

                // remove mined transactions from pending transactions
                List<string> minedTxIds = minedBlock.Transactions.Select(t => t.TransactionHash).ToList();
                PendingTransactions = new ConcurrentBag<Transaction>(PendingTransactions.Where(t => !minedTxIds.Contains(t.TransactionHash)).ToList());

                BlockChain.TryAdd(minedBlock.Index, minedBlock);
            }
        }

        private void RevalidateBlock(Block b)
        {
            if (!b.IsDataValid())
                throw new ArgumentException($"Block datac changed by middle man.Index={b.Index}");

            foreach (var tx in b.Transactions)
            {
                ValidateTransaction(tx);
                ulong senderBalance = CalculateBalance(tx.FromAddress, false, true);
                bool isSuccesfullRevalidate = senderBalance >= tx.Amount;
                if (tx.TranserSuccessfull != isSuccesfullRevalidate)
                    throw new ArgumentException($"Block not valid. Transaction {tx.TransactionHash} changedby middle man.");
            }
        }
    }
}
