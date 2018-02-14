using System;
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
                return Ok(result);
            }

            return NotFound($"Block with index {index} is not found");

        }

        [HttpGet("/new/{index:int}/{peer}")]
        public void NewBlockFound(int index, string peer)
        {
            //new block foudn by peer
        }

        [HttpDelete("/balance/{address:string}")]
        public IActionResult Balance(string address)
        {
            decimal balance = Node.GetBalance(address);
            return Ok(balance);
        }
    }
}