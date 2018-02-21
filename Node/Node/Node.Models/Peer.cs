
namespace Node.Domain
{
    public class Peer
    {
        public string Url { get; private set; }

        public string Name { get; private set; }


        public Peer(string url, string name)
        {
            Url = url;
            Name = name;
        }
    }
}
