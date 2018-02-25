using System;
using BlockChain.Core;

namespace Node.Domain
{
    public class BlockApiModel
    {
        public int Index { get; set; }
        public string PreviousBlockHash { get; set; }
        public int Difficulty { get; set; }
        public string MinedBy { get; set; }
        public string BlockDataHash { get; set; }
        public string BlockHash { get; set; }
        public int Nonce { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TransactionsCount { get; set; }
        public string BlockReward { get; set;  }

        public static BlockApiModel FromBlock(Block block)
        {
            return new BlockApiModel() {
                Index = block.Index,
                BlockDataHash = block.BlockDataHash,
                BlockHash = block.BlockHash,
                CreatedDate = block.CreatedDate,
                Difficulty = block.Difficulty,
                MinedBy = block.MinedBy,
                Nonce = block.Nonce,
                PreviousBlockHash = block.PreviousBlockHash,
                TransactionsCount = block.Transactions.Count,
                BlockReward = block.BlockReward.GetFormattedTokens()
            };
        }
    }
}
