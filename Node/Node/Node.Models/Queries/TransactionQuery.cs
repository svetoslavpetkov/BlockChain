using Node.Domain.ApiModels;
using System.Collections.Generic;
using System.Linq;

namespace Node.Domain
{
    public interface ITransactionQuery
    {
        GetTransactionApiModel Get(string txHash);
        IEnumerable<GetTransactionApiModel> GetPending();
        IEnumerable<GetTransactionApiModel> GetTransactions(string address, int count);
        IEnumerable<GetTransactionApiModel> GetBlcokTransactions(int index);
    }

    public class TransactionQuery : ITransactionQuery
    {
        private Node Node { get; set; }

        public TransactionQuery(Node node)
        {
            Node = node;
        }

        public GetTransactionApiModel Get(string txHash)
        {
            GetTransactionApiModel result = Node.PendingTransactions
                .Where(t => t.TransactionHash == txHash)
                .Select(t => GetTransactionApiModel.FromTransaction(t))
                .FirstOrDefault();

            if (result == null)
            {
                result = Node.BlockChain
                    .SelectMany(b => b.Value.Transactions.Select(t => new { BlockIndex = b.Key, Transaction = t }))
                    .Where(t => t.Transaction.TransactionHash == txHash)
                    .Select(x => GetTransactionApiModel.FromTransaction(x.Transaction, x.BlockIndex))
                    .FirstOrDefault();
            }

            return result;
        }

        public IEnumerable<GetTransactionApiModel> GetPending()
        {
            return Node.PendingTransactions.Select(p => GetTransactionApiModel.FromTransaction(p));
        }

        public IEnumerable<GetTransactionApiModel> GetTransactions(string address, int count)
        {
            IEnumerable< GetTransactionApiModel> txs=  Node.GetTransactions(address, true)
                         .OrderByDescending(tx => tx.DateCreated)
                         .Take(count)
                         .Select(tx => GetTransactionApiModel.FromTransaction(tx))
                         .ToList();

            return txs;
        }

        public IEnumerable<GetTransactionApiModel> GetBlcokTransactions(int index)
        {
            bool success = Node.BlockChain.TryGetValue(index, out Block block);

            List<GetTransactionApiModel> txs = new List<GetTransactionApiModel>();

            if (success)
            {
               txs = block.Transactions.Select(tx => GetTransactionApiModel.FromTransaction(tx)).ToList();
               return txs;
            }

            return null;
        }
    }
}
