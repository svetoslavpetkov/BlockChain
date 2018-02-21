using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain.ApiModels
{
    public class BlockApiModel
    {
        public int Index { get; private set; }
        //public ICollection<GetTransactionApiModel> Transactions { get; set; } = new List<GetTransactionApiModel>();
        public int Difficulty { get; private set; }
        public string PreviousBlockHash { get; private set; }
        public string MinedBy { get; private set; }
        public string BlockDataHash { get; private set; }
        /// <summary>
        /// Used to store hash calculated from miner    
        /// </summary>
        public string BlockHash { get; private set; }

        public int Nonce { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public int TransactionsCount { get; set; }


        public static BlockApiModel FromBlock(Block block)
        {
            return new BlockApiModel() {
                Index = block.Index,
                //Transactions = block.Transactions.Select(tx => GetTransactionApiModel.FromTransaction(tx)).ToList(),
                BlockDataHash = block.BlockDataHash,
                BlockHash = block.BlockHash,
                CreatedDate = block.CreatedDate,
                Difficulty = block.Difficulty,
                MinedBy = block.MinedBy,
                Nonce = block.Nonce,
                PreviousBlockHash = block.PreviousBlockHash,
                TransactionsCount = block.Transactions.Count
            };
        }
    }
}
