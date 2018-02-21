using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain.ApiModels
{
    public class PeerApiModel : Peer
    {
        public PeerApiModel(string url, string name)
            : base(url, name)
        {

        }

        public static PeerApiModel FromPeer(Peer peer)
        {
            return new PeerApiModel(peer.Url, peer.Name);
        }


    }
}
