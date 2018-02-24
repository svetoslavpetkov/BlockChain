using System.Collections.Generic;
using System.Linq;

namespace Node.Domain
{
    public interface IBlockQuery
    {
        BlockApiModel Get(int index);
        BlockApiModel GetLastBlock();
        List<BlockApiModel> GetBlocks(int fromIndex, int count);
        List<BlockSyncApiModel> GetBlocksForSync(int fromIndex, int count);
    }

    public class BlockQuery : IBlockQuery
    {
        private Node Node { get; set; }

        public BlockQuery(Node node)
        {
            Node = node;
        }

        public List<BlockSyncApiModel> GetBlocksForSync(int fromIndex, int count)
        {
            List<BlockSyncApiModel> blocks = new List<BlockSyncApiModel>();
            int endIndex = fromIndex + count;

            if (endIndex > Node.BlockChain.Count)
                endIndex = Node.BlockChain.Count;

            for (int i = fromIndex; i < endIndex; i++)
                blocks.Add(BlockSyncApiModel.FromBlock(Node.BlockChain[i]));

            return blocks;
        }

        public BlockApiModel Get(int index)
        {
            Block result;
            bool success = Node.BlockChain.TryGetValue(index, out result);
            BlockApiModel blockApiModel = BlockApiModel.FromBlock(result);

            return blockApiModel;
        }

        public BlockApiModel GetLastBlock()
        {
            BlockApiModel lastBlock = BlockApiModel.FromBlock(Node.LastBlock);

            return lastBlock;
        }

        public List<BlockApiModel> GetBlocks(int fromIndex, int count)
        {
            int endIndex = fromIndex + count;
            if (endIndex > Node.BlockChain.Count)
                endIndex = Node.BlockChain.Count -1;

            List<BlockApiModel> result = new List<BlockApiModel>();
            for (int i = fromIndex; i <= endIndex; i++)
                result.Add(BlockApiModel.FromBlock(Node.BlockChain[i]));

            result = result.OrderByDescending(b => b.Index).ToList();

            return result;
        }
    }
}
