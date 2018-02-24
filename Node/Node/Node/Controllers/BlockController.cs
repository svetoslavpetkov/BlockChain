using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Node.Domain;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/block")]
    public class BlockController : Controller
    {
        private ITransactionQuery TransactionQuery { get; set; }
        private IBlockQuery BlockQuery { get; set; }
        private Domain.Node Node { get; set; }

        public BlockController( ITransactionQuery transactionQuery, IBlockQuery blockQuery, Domain.Node node)
        {
            TransactionQuery = transactionQuery;
            BlockQuery = blockQuery;
            Node = node;
        }

        [HttpGet("{index}")]
        public IActionResult Get(int index)
        {
            BlockApiModel block = BlockQuery.Get(index);

            if (block != null)
                return Ok(block);

            return NotFound($"Block with index {index} is not found");
        }

        [HttpGet("{index}/transactions")]
        public IActionResult GetBlockTransactions(int index)
        {
            IEnumerable < GetTransactionApiModel > txs =  TransactionQuery.GetBlcokTransactions(index);

            if (txs != null)
                return Ok(txs);

            return NotFound($"Block with index {index} is not found");
        }

        [HttpGet("last")]
        public IActionResult GetLastBlock()
        {
            BlockApiModel lastBlock = BlockQuery.GetLastBlock();

            return Ok(lastBlock);
        }

        [HttpGet("getblocksByFromIndexAndCount/{fromIndex}/{count}")]
        public IActionResult GetBlocks(int fromIndex, int count)
        {
            if(fromIndex < 0 || count < 0)
                return BadRequest("'formIndex' and 'count' must be positive.");

            List<BlockApiModel> result = BlockQuery.GetBlocks(fromIndex, count);

            return Ok(result);
        }

        [HttpGet("getBlocksForSync/{fromIndex}/{count}")]
        public List<BlockSyncApiModel> GetBlocksForSync(int fromIndex, int count)
        {
            List<BlockSyncApiModel> blocks = BlockQuery.GetBlocksForSync(fromIndex, count);
            return blocks;
        }

        [HttpPost("/new")]
        public void NewBlockFound(NewBlockApiModel blockInfo)
        {

            Node.AttachBroadcastedBlock(blockInfo.Block,blockInfo.NodeAddress);
        }
    }
}