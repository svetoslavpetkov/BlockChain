using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Node.Domain;
using Node.Domain.ApiModels;

namespace Node.Controllers
{
    [Produces("application/json")]
    [Route("api/peers")]
    public class PeersController : Controller
    {
        private INodeSynchornizator NodeSynchornizator { get; set; }

        public PeersController(INodeSynchornizator nodeSynchornizator)
        {
            NodeSynchornizator = nodeSynchornizator;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(NodeSynchornizator.Peers.Select(p => PeerApiModel.FromPeer(p)).ToList());
        }

        [HttpPost("connect")]
        public PeerApiModel Connect(PeerApiModel peer)
        {
           PeerApiModel thisPeer =  NodeSynchornizator.AddNewlyConnectedPeer(peer);

            return thisPeer;
        }
    }
}