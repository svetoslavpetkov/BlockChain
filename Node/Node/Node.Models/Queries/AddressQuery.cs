
namespace Node.Domain
{
    public interface IAddressQuery
    {
        ulong GetBalance(string address);
        ulong GetUnconfirmedBalance(string address);
    }

    public class AddressQuery : IAddressQuery
    {
        private Node Node { get; set; }

        public AddressQuery(Node node)
        {
            Node = node;
        }

        public ulong GetBalance(string address)
        {
            ulong balance = Node.CalculateBalance(address, false, true);
            return balance;
        }

        public ulong GetUnconfirmedBalance(string address)
        {
            ulong unconfirmedBalance = Node.CalculateBalance(address, true, true);
            return unconfirmedBalance;
        }
    }
}
