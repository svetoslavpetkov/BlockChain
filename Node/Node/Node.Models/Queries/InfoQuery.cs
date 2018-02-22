using Node.Domain.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Node.Domain
{
    public interface IInfoQuery
    {
        NodeInfoApiModel GetInfo();
    }

    public class InfoQuery : IInfoQuery
    {
        private Node Node { get; set; }

        public InfoQuery(Node node)
        {
            Node = node;
        }

        public NodeInfoApiModel GetInfo()
        {
            NodeInfoApiModel result = new NodeInfoApiModel()
            {
                About = Node.NodeInfo.About,
                Name = Node.NodeInfo.Name,
                ConfirmedTransactions = Node.BlockChain.Select(b => b.Value.Transactions.Count).Sum(),
                PendingTransactions = Node.PendingTransactions.Count,
                Difficulty = Node.Difficulty,
                Peers = Node.Peers.Count,
                Started = Node.Started
            };

            return result;
        }
    }
}
