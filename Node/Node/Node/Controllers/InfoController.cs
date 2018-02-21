using Microsoft.AspNetCore.Mvc;
using Node.Domain.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/info")]
    public class InfoController : Controller
    {
        private Domain.Node Node { get; set; }

        public InfoController(Domain.Node node)
        {
            Node = node;
        }


        [HttpGet]
        public IActionResult Get()
        {
            NodeInfoApiModel result = new NodeInfoApiModel() {
                About = Node.About,
                Name = Node.Name,
                ConfirmedTransactions = Node.BlockChain.Select(b => b.Value.Transactions.Count).Sum(),
                PendingTransactions = Node.PendingTransactions.Count,
                Difficulty = Node.Difficulty,
                Peers = Node.Peers.Count,
                Started = Node.Started
            };
            
            return Ok(result);
        }
    }
}
