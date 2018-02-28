using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Node.Domain;

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

        [HttpPost("clear")]
        public IActionResult Clear()
        {
            NodeSynchornizator.ClearPeers();
            return Ok();
        }

        [HttpPost("sync")]
        public IActionResult Sync()
        {
            NodeSynchornizator.SyncPeers();
            return Ok();
        }

        [HttpPost("connect")]
        public PeerApiModel Connect([FromBody] PeerApiModel peer)
        {
           PeerApiModel thisPeer =  NodeSynchornizator.AddNewlyConnectedPeer(peer);

            return thisPeer;
        }
    }
}