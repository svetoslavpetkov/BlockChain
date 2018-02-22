﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Node.Domain;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/block")]
    public class BlockController : Controller
    {
        private Domain.Node Node { get; set; }
        public BlockController(Domain.Node node)
        {
            Node = node;
        }

        [HttpGet("{index}")]
        public IActionResult Get(int index)
        {
            Block result;
            bool success = Node.BlockChain.TryGetValue(index, out result);

            if (success)
            {
                return Ok(Domain.ApiModels.BlockApiModel.FromBlock(result));
            }

            return NotFound($"Block with index {index} is not found");
        }


        [HttpGet("{index}/transactions")]
        public IActionResult GetBlockTransactions(int index)
        {
            Block result;
            bool success = Node.BlockChain.TryGetValue(index, out result);

            if (success)
            {
                return Ok(result
                            .Transactions
                            .Select(tx=> Domain.ApiModels.GetTransactionApiModel.FromTransaction(tx))
                            .ToList());
            }

            return NotFound($"Block with index {index} is not found");
        }

        [HttpGet("last")]
        public IActionResult GetLastBlock()
        {
            return Ok(Domain.ApiModels.BlockApiModel.FromBlock(Node.BlockChain[Node.BlockChain.Count - 1]));
        }

        [HttpGet("getblocksByFromIndexAndCount/{fromIndex}/{count}")]
        public IActionResult GetBlocks(uint fromIndex, uint count)
        {
            int from = (int)fromIndex;
            int to = from + (int)count;

            //if (from < 0)
            //{
            //    return NotFound($"Blocks starting from index {from} are not existing");
            //}

            //Itterate backwards
            List<Domain.ApiModels.BlockApiModel> result = new List<Domain.ApiModels.BlockApiModel>();
            for (int i = from; i > to; i--)
            {
                result.Add(Domain.ApiModels.BlockApiModel.FromBlock(Node.BlockChain[(int)i]));
            }
            return Ok(result);
        }

        [HttpGet("getBlocksForSync/{fromIndex}/{count}")]
        public List<BlockSyncApiModel> GetBlocksForSync(int fromIndex, int count)
        {
            List<BlockSyncApiModel> blocks = new List<BlockSyncApiModel>();
            int endIndex = fromIndex + count;
            for (int i = fromIndex; i < endIndex; i++)
            {
                blocks.Add(BlockSyncApiModel.FromBlock(Node.BlockChain[i]));
            }
            return blocks;
        }

        [HttpPost("/new")]
        public void NewBlockFound(NewBlockApiModel blockInfo)
        {
            Node.AttachBroadcastedBlock(blockInfo.Block,blockInfo.NodeAddress);
        }
    }
}