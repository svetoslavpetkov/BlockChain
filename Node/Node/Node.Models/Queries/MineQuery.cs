
namespace Node.Domain
{
    public interface IMineQuery
    {
        MiningContext GetBlockForMine(string minerAddress);
    }

    public class MineQuery : IMineQuery
    {
        private Node Node { get; set; }

        public MineQuery(Node node)
        {
            Node = node;
        }

        public MiningContext GetBlockForMine(string minerAddress)
        {
            MiningContext jobForMine = Node.GetBlockForMine(minerAddress);

            return jobForMine;
        }
    }
}
