﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Node.Domain;

namespace Node.Controllers
{

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
            List<BlockSyncApiModel> blocks = BlockQuery.All();
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
                return AsJson(txs);

            return NotFound($"Block with index {index} is not found");
        }

        [HttpGet("last/{count}")]
        public IActionResult GetLastBlock(int count)
        {
            List<BlockApiModel> lastBlock = BlockQuery.GetLastBlocks(count);
            return Ok(lastBlock);
        }


        [HttpGet("all")]
        public IActionResult GettAllBlocks()
        {
            List<BlockApiModel> lastBlock = BlockQuery.GetAllBlocks();
            return Ok(lastBlock);
        }

        [HttpGet("getblocksByFromIndexAndCount/{fromIndex}/{count}")]
        public IActionResult GetBlocks(int fromIndex, int count)
        {
            if ( count < 0)
                return BadRequest("'count' must be positive.");

            if (fromIndex < 0)
                fromIndex = 0;

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
            var blData = JsonConvert.SerializeObject(blockInfo);
            Console.WriteLine("Block for sync: " + blData);
            Console.WriteLine();
            Console.WriteLine();

            Node.AttachBroadcastedBlock(blockInfo.Block, blockInfo.NodeAddress);

        }
    }
}