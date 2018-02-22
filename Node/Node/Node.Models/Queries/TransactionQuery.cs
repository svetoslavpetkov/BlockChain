using Node.Domain.ApiModels;
using System.Collections.Generic;
using System.Linq;

namespace Node.Domain
{
    public interface ITransactionQuery
    {
        GetTransactionApiModel Get(string txHash);
        IEnumerable<GetTransactionApiModel> GetPending();
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
    }
}
