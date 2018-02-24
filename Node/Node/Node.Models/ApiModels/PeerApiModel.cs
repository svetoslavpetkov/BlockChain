
namespace Node.Domain
{
    public class PeerApiModel
    {
        public string Url { get; set; }

        public string Name { get; set; }

        public static PeerApiModel FromPeer(Peer peer)
        {
            return new PeerApiModel() { Url = peer.Url, Name = peer.Name };
        }
    }
}
