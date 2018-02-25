using System.Collections.Generic;
using System.Linq;

namespace Node.Domain
{
    public interface IBlockQuery
    {
        BlockApiModel Get(int index);
        List<BlockApiModel> All();
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
            bool success = Node.BlockChain.TryGetValue(index, out Block result);
            if (success)
                return BlockApiModel.FromBlock(result);

            return null;
        }

        public List<BlockApiModel> All()
        {
            List<BlockApiModel> blocks = new List<BlockApiModel>();
            foreach (var b in Node.BlockChain)
                blocks.Add(BlockApiModel.FromBlock(b.Value));
            blocks = blocks.OrderByDescending(b => b.Index).ToList();

            return blocks;
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
                endIndex = Node.BlockChain.Count - 1;

            List<BlockApiModel> result = new List<BlockApiModel>();
            for (int i = fromIndex; i <= endIndex; i++)
                result.Add(BlockApiModel.FromBlock(Node.BlockChain[i]));

            result = result.OrderByDescending(b => b.Index).ToList();

            return result;
        }
    }
}
