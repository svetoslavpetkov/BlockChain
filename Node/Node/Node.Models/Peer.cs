
namespace Node.Domain
{
    public class Peer
    {
        public string Url { get; private set; }

        public Peer(string url)
        {
            Url = url;
        }
    }
}
