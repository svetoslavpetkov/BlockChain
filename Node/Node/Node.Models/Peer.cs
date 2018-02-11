
namespace Node.Models
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
