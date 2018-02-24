using BlockChain.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Node.Domain
{
    public class BlockSyncApiModel
    {
        public int Index { get; set; }
        public List<Transaction> Transactions { get; set; }
        public int Difficulty { get; set; }
        public string PreviousBlockHash { get; set; }
        public string MinedBy { get; set; }
        public string BlockDataHash { get; set; }
        public string BlockHash { get; set; }
        public int Nonce { get; set; }
        public DateTime CreatedDate { get; set; }

        public static BlockSyncApiModel FromBlock(Block block)
        {
            BlockSyncApiModel blockModel = new BlockSyncApiModel();
            blockModel.Index = block.Index;
            blockModel.Transactions = block.Transactions.ToList();
            blockModel.Difficulty = block.Difficulty;
            blockModel.PreviousBlockHash = block.PreviousBlockHash;
            blockModel.MinedBy = block.MinedBy;
            blockModel.BlockDataHash = block.BlockDataHash;
            blockModel.BlockHash = block.BlockHash;
            blockModel.Nonce = block.Nonce;
            blockModel.CreatedDate = block.CreatedDate;

            return blockModel;
        }
    }
}
