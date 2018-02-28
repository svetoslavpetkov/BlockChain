using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Node.Domain;
using Node.Domain.ApiModels;
using Node.Domain.Exceptions;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/mining")]
    public class MiningController : Controller
    {
        private Domain.Node Node { get; set; }
        private IMineQuery MineQuery { get; set; }

        public MiningController(Domain.Node node, IMineQuery mineQuery)
        {
            Node = node;
            MineQuery = mineQuery;
        }

        [HttpGet("getBockForMine/{minerAddress}")]
        public IActionResult GetBlockForMine(string minerAddress)
        { 
            MiningContext context = MineQuery.GetBlockForMine(minerAddress);

            return Ok(context);
        }

        [HttpPost("noncefound")]
        public IActionResult NonceFound([FromBody]BlockMinedApiModel blockMinedRequest)
        {
            try
            {
                Node.NonceFound(blockMinedRequest.MinerAddress, blockMinedRequest.Nonce, blockMinedRequest.Hash);
                return Ok();
            }
            catch (NonceUselessException)
            {
                return BadRequest();
            }

        }
    }
}