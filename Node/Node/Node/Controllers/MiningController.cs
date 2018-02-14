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
    [Route("api/mining")]
    public class MiningController : Controller
    {
        private Domain.Node Node { get; set; }
        public MiningController(Domain.Node node)
        {
            Node = node;
        }

        [HttpGet("getBockForMine")]
        public IActionResult GetBlockForMine(string minerAddress)
        { 
            MiningContext context = Node.GetBlockForMine(minerAddress);

            return Ok(context);
        }
    }
}