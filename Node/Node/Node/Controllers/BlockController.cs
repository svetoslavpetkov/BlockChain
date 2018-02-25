﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Node.Domain;

namespace Node.Controllers
{
    public class ContorlerBase : Controller
    {
        protected JsonResult AsJson(object data)
        {
            JsonSerializerSettings serializerSettings = 
                new JsonSerializerSettings { Formatting = Formatting.Indented };
            return Json(data, serializerSettings);
        }
    }
    [Produces("application/json")]
    [Route("api/block")]
    public class BlockController : ContorlerBase
    {
        private ITransactionQuery TransactionQuery { get; set; }
        private IBlockQuery BlockQuery { get; set; }
        private Domain.Node Node { get; set; }

        public BlockController(ITransactionQuery transactionQuery, IBlockQuery blockQuery, Domain.Node node)
        {
            TransactionQuery = transactionQuery;
            BlockQuery = blockQuery;
            Node = node;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            List<BlockApiModel> blocks = BlockQuery.All();
            return AsJson(blocks);
         
        }

        [HttpGet("{index}")]
        public IActionResult Get(int index)
        {
            BlockApiModel block = BlockQuery.Get(index);

            if (block != null)
                return AsJson(block);

            return NotFound($"Block with index {index} is not found");
        }

        [HttpGet("{index}/transactions")]
        public IActionResult GetBlockTransactions(int index)
        {
            IEnumerable<GetTransactionApiModel> txs = TransactionQuery.GetBlcokTransactions(index);

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
            if (fromIndex < 0 || count < 0)
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

        [HttpPost("new")]
        public void NewBlockFound([FromBody]NewBlockApiModel blockInfo)
        {

            Node.AttachBroadcastedBlock(blockInfo.Block, blockInfo.NodeAddress);
        }
    }
}