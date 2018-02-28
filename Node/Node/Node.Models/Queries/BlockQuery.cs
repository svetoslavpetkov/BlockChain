using System.Collections.Generic;
using System.Linq;

namespace Node.Domain
{
    public interface IBlockQuery
    {
        BlockApiModel Get(int index);
        List<BlockSyncApiModel> All();
        List<BlockApiModel> GetLastBlocks(int count);
        List<BlockApiModel> GetBlocks(int fromIndex, int count);
        List<BlockSyncApiModel> GetBlocksForSync(int fromIndex, int count);
        List<BlockApiModel> GetAllBlocks();
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
            int endIndex = fromIndex + count - 1;

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

        public List<BlockSyncApiModel> All()
        {
            List<BlockSyncApiModel> blocks = new List<BlockSyncApiModel>();
            foreach (var b in Node.BlockChain)
                blocks.Add(BlockSyncApiModel.FromBlock(b.Value));

            return blocks;
        }

        public List<BlockApiModel> GetLastBlocks(int count)
        {
            int startFrom = Node.BlockChain.Count - count;

            if (startFrom < 0)
                startFrom = 0;

            List<Block> blocks = Node.BlockChain.Values.OrderByDescending(b => b.Index).Take(count).ToList();
            List<BlockApiModel> result =  blocks.Select(b => BlockApiModel.FromBlock(b)).ToList();

            return result;
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

        public List<BlockApiModel> GetAllBlocks()
        {
            List<Block> blocks = Node.BlockChain.Values.OrderByDescending(b => b.Index).ToList();
            List<BlockApiModel> result = blocks.Select(b => BlockApiModel.FromBlock(b)).ToList();

            return result;
        }
    }
}
