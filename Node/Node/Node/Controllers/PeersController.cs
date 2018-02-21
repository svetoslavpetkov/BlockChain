using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Node.Domain;
using Node.Domain.ApiModels;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/peers")]
    public class PeersController : Controller
    {
        private Domain.INodeSynchornizator NodeSynchornizator { get; set; }

        public PeersController(Domain.Node node, INodeSynchornizator nodeSynchornizator)
        {
            NodeSynchornizator = nodeSynchornizator;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(NodeSynchornizator.Peers.Select(p => PeerApiModel.FromPeer(p)).ToList());
        }

        [HttpPost("/connect")]
        public IActionResult Connect(PeerApiModel peer)
        {
            NodeSynchornizator.AddNewlyConnectedPeer(new Peer(peer.Url, peer.Url));
            return Ok();
        }
    }
}