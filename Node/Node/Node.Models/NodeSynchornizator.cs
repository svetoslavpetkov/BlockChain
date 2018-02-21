using BlockChain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Domain
{
    public interface INodeSynchornizator
    {
        List<Block> Sync();
        void BroadcastTransaction(Transaction tx);
        void BroadcastBlock(Block block);
    }

    public class NodeSynchornizator : INodeSynchornizator
    {
        private class NodeDetails
        {
            public string Name { get; set; }
        }

        private List<Peer> Peers { get; set; }

        public NodeSynchornizator()
        {
            Peers = new List<Peer>();
            DiscoverPeers();
        }

        private void DiscoverPeers()
        {
            for (int port = 5500; port < 5600; port++)
            {
                string url = $"http://localhost:{port}";
                RestClient cl = new RestClient(url);
                NodeDetails nodeDetails = cl.Get<NodeDetails>("api/info");
                if (nodeDetails != null)
                    Peers.Add(new Peer(url, nodeDetails.Name));
            }
        }

        public List<Block> Sync()
        {
            return null;
        }

        public void BroadcastTransaction(Transaction tx)
        {
            foreach (Peer p in Peers)
            {
                try
                {
                    RestClient client = new RestClient(p.Url);
                    client.Post<Transaction>("api/transaction/new", tx);
                }
                catch (Exception) { }
            }
        }

        public void BroadcastBlock(Block block)
        {
            foreach (Peer p in Peers)
            {
                try
                {
                    RestClient client = new RestClient(p.Url);
                    client.Post("api/block/new", block);
                }
                catch (Exception) { }
            }
        }
    }
}
