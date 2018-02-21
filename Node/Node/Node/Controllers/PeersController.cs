using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Node.Domain.ApiModels;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/peers")]
    public class PeersController : Controller
    {
        private Domain.Node Node { get; set; }
        public PeersController(Domain.Node node)
        {
            Node = node;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(Node.Peers.Select(p => PeerApiModel.FromPeer(p)).ToList());
        }

        [HttpPost("/connect")]
        public IActionResult Connect(PeerApiModel peer)
        {
            return Ok();
        }
    }
}